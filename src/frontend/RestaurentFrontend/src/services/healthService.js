import axiosInstance from "../axios/axiosInstance";

const checkHealth = async () => {
  const response = await axiosInstance.get(`/health`);
  return response.data;
};

export default {
  checkHealth,
};
