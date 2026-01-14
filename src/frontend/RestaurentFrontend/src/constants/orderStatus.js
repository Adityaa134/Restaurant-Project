export const ORDER_STATUS = {
  PENDING: 0,
  PREPARING: 1,
  CONFIRMED: 2,
  CANCELLED: 3,
  DELIVERED: 4
};

export const ORDER_STATUS_LABEL = {
  0: "Pending",
  1: "Preparing",
  2: "Confirmed",
  3: "Cancelled",
  4: "Delivered"
};

export const ORDER_STATUS_FLOW = [
  ORDER_STATUS.PENDING,
  ORDER_STATUS.PREPARING,
  ORDER_STATUS.CONFIRMED,
  ORDER_STATUS.DELIVERED
];

export const ORDER_STATUS_COLOR = {
  0: "bg-yellow-100 text-yellow-800",   
  1: "bg-purple-100 text-purple-800",  
  2: "bg-blue-100 text-blue-800",      
  3: "bg-red-100 text-red-800",       
  4: "bg-green-100 text-green-800"     
};