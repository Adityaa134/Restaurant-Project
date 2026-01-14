import { useState} from "react";
import AddressSection from "./AddressSection";
import OrderSummary from "./OrderSummary";

const Checkout = () => {
  const [selectedAddress, setSelectedAddress] = useState(null);

  return (
    <div className="bg-gray-50 min-h-screen py-6 px-4">
      <div className="max-w-5xl mx-auto space-y-6">
        <h1 className="text-2xl font-bold">Checkout</h1>

        <AddressSection
          selectedAddress={selectedAddress}
          setSelectedAddress={setSelectedAddress}
        />

        <OrderSummary
        selectedAddress={selectedAddress}
        isAddressSelected={!!selectedAddress}
        />
      </div>
    </div>
  );
};

export default Checkout;