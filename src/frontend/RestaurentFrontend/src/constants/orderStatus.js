export const ORDER_STATUS = {
  PENDING: 0,
  PREPARING: 1,
  CONFIRMED: 2,
  CANCELLED: 3,
  DELIVERED: 4,
};

export const ORDER_STATUS_LABEL = {
  0: "Pending",
  1: "Preparing",
  2: "Confirmed",
  3: "Cancelled",
  4: "Delivered",
};

export const ORDER_STATUS_FLOW = [
  ORDER_STATUS.PENDING,
  ORDER_STATUS.PREPARING,
  ORDER_STATUS.CONFIRMED,
  ORDER_STATUS.DELIVERED,
];

export const ORDER_STATUS_COLOR = {
  0: "bg-yellow-50 text-yellow-700 border border-yellow-200",
  1: "bg-orange-50 text-orange-700 border border-orange-200",
  2: "bg-blue-50 text-blue-700 border border-blue-200",
  3: "bg-red-50 text-red-700 border border-red-200",
  4: "bg-green-50 text-green-700 border border-green-200",
};
