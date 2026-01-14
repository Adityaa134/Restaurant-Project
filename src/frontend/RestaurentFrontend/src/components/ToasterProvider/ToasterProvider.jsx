import { Toaster } from "react-hot-toast";

const ToastProvider = () => {
  return (
    <Toaster
      position="top-right"
      toastOptions={{
        duration: 2000,
        style: {
          fontSize: "14px",
        },
      }}
    />
  );
};

export default ToastProvider;