import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Input, Button } from "../index";

const MAX_LENGTH = 40;

const AddressForm = ({
  initialData,
  onCancel,
  onSubmit,
  loading
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isValid }
  } = useForm({
    mode: "onChange",
    defaultValues: {
      addressLine: "",
      city: "",
      area: "",
      landmark: ""
    }
  });

  useEffect(() => {
    if (initialData) {
      reset({
        addressLine: initialData.addressLine || "",
        city: initialData.city || "",
        area: initialData.area || "",
        landmark: initialData.landmark || ""
      });
    }
  }, [initialData, reset]);

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="border rounded-xl p-4 bg-gray-50 space-y-4"
    >
      <h3 className="font-semibold text-gray-800">
        {initialData ? "Edit Address" : "Add New Address"}
      </h3>

      <div>
        <Input
          placeholder="Address Line"
          className="border rounded p-2"
          {...register("addressLine", {
            required: "Address line is required",
            maxLength: {
              value: MAX_LENGTH,
              message: "Maximum 40 characters allowed"
            }
          })}
        />
        {errors.addressLine && (
          <p className="text-xs text-red-500 mt-1">
            {errors.addressLine.message}
          </p>
        )}
      </div>

      <div>
        <Input
          placeholder="Area"
          className="border rounded p-2"
          {...register("area", {
            required: "Area is required",
            maxLength: {
              value: MAX_LENGTH,
              message: "Maximum 40 characters allowed"
            }
          })}
        />
        {errors.area && (
          <p className="text-xs text-red-500 mt-1">
            {errors.area.message}
          </p>
        )}
      </div>

      <div>
        <Input
          placeholder="City"
          className="border rounded p-2"
          {...register("city", {
            required: "City is required",
            maxLength: {
              value: MAX_LENGTH,
              message: "Maximum 40 characters allowed"
            }
          })}
        />
        {errors.city && (
          <p className="text-xs text-red-500 mt-1">
            {errors.city.message}
          </p>
        )}
      </div>

      <div>
        <Input
          placeholder="Landmark (optional)"
          className="border rounded p-2"
          {...register("landmark", {
            maxLength: {
              value: MAX_LENGTH,
              message: "Maximum 40 characters allowed"
            }
          })}
        />
        {errors.landmark && (
          <p className="text-xs text-red-500 mt-1">
            {errors.landmark.message}
          </p>
        )}
      </div>

      <div className="flex justify-end gap-3">
        <Button
          type="button"
          onClick={onCancel}
          className="text-sm text-gray-600"
        >
          Cancel
        </Button>

        <Button
          type="submit"
          disabled={!isValid || loading}
          className={`
            text-sm font-medium transition
            ${isValid
              ? "bg-green-600 text-white hover:bg-green-700"
              : "bg-gray-300 text-gray-500 cursor-not-allowed"}
          `}
        >
          {loading ? "Saving..." : initialData ? "Update" : "Save"}
        </Button>
      </div>
    </form>
  );
};

export default AddressForm;