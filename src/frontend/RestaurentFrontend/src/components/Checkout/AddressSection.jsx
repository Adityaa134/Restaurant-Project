import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import addressService from "../../services/addressService";
import AddressForm from "./AddressForm";

const AddressSection = ({ selectedAddress, setSelectedAddress }) => {
  const userId = useSelector((state) => state.auth.userData?.userId);
  const [addresses, setAddresses] = useState([]);
  const [addressLoading, setAddressLoading] = useState(false);
  const [editingAddress, setEditingAddress] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const fetchAddresses = async () => {
      try {
        setAddressLoading(true)
        const response = await addressService.GetAddressesOfUser(userId);
        setAddresses(Array.isArray(response) ? response : []);
      } catch (error) {
        console.log(error)
      } finally {
        setAddressLoading(false)
      }
    }
    fetchAddresses();
  }, [userId]);

  const handleSubmit = async (formData) => {
    try {
      setSaving(true);

      let result;
      if (editingAddress) {
        result = await addressService.UpdateAddress({
          addressId: editingAddress.addressId,
          userId,
          ...formData
        });
        setAddresses((prev) =>
          prev.map((a) =>
            a.addressId === result.addressId ? result : a
          )
        );
      } else {
        result = await addressService.CreateAddress({
          userId,
          ...formData
        });

        setAddresses((prev) => [...prev, result]);
      }

      setSelectedAddress(result);
      setShowForm(false);
      setEditingAddress(null);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="bg-white rounded-xl shadow p-5 space-y-4">
      <div className="flex justify-between items-center">
        <h2 className="font-semibold">Delivery Address</h2>
        <button
          onClick={() => {
            setEditingAddress(null);
            setShowForm(true);
          }}
          className="text-blue-600 font-medium"
        >
          + Add New
        </button>
      </div>

      {addressLoading && (
        <div className="space-y-4 animate-pulse">
          {[1, 2, 3].map((_, i) => (
            <div
              key={i}
              className="flex items-start gap-3 border rounded-lg p-3"
            >
              <div className="w-4 h-4 mt-1 bg-gray-300 rounded-full" />
              <div className="flex-1 space-y-2">
                <div className="h-4 bg-gray-300 rounded w-3/4" />
                <div className="h-3 bg-gray-200 rounded w-1/2" />
              </div>
              <div className="w-4 h-4 bg-gray-300 rounded" />
            </div>
          ))}
        </div>
      )}

      {!addressLoading && addresses.length === 0 && (
        <p className="text-sm text-gray-500">
          No saved addresses. Please add one.
        </p>
      )}

      {!addressLoading &&
        addresses.map((addr) => (
          <label
            key={addr.addressId}
            className={`
              flex gap-3 border p-3 rounded cursor-pointer
              ${selectedAddress?.addressId === addr.addressId
                ? "border-green-500 bg-green-50"
                : "hover:border-green-400"}
            `}
          >
            <input
              type="radio"
              checked={selectedAddress?.addressId === addr.addressId}
              onChange={() => setSelectedAddress(addr)}
            />
            <div className="flex-1">
              <p className="font-medium">{addr.addressLine}</p>
              <p className="text-sm text-gray-500">
                {addr.area}, {addr.city}
              </p>
            </div>
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                setEditingAddress(addr);
                setShowForm(true);
              }}
              className="text-gray-500 hover:text-gray-700"
            >
              ✏️
            </button>
          </label>
        ))}
        
      {showForm && (
        <AddressForm
          initialData={editingAddress}
          onSubmit={handleSubmit}
          onCancel={() => setShowForm(false)}
          loading={saving}
        />
      )}
    </div>
  );
};

export default AddressSection;