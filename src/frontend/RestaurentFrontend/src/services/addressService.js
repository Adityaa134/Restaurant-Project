import axiosInstance from "../axios/axiosInstance"

export class Address {

    async GetAddressesOfUser(userId) {
        try {
            let response = await axiosInstance.get(`/api/Address/user-address/${userId}`)
            return response.data
        }
        catch (error) {
            logger.log("AddressService :: GetAddressesOfUser :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async GetAddressById(addressId) {
        try {
            let response = await axiosInstance.get(`/api/Address/${addressId}`)
            return response.data
        } catch (error) {
            logger.log("AddressService :: GetAddressById :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async CreateAddress({userId, addressLine, city, landmark, area}) {
        try {
            const response = await axiosInstance.post(`/api/Address`,
                {
                    UserId:userId,
                    AddressLine:addressLine,
                    City:city,
                    Landmark:landmark,
                    Area:area
                }
            );
            return response.data;
        } catch (error) {
            logger.log("AddressService :: CreateAddress :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }

    async UpdateAddress({addressId, userId, addressLine, city, landmark, area}) {
        try {
            const response = await axiosInstance.put(`/api/Address`,{
                    AddressId:addressId,   
                    UserId:userId,
                    AddressLine:addressLine,
                    City:city,
                    Landmark:landmark,
                    Area:area
                });

            return response.data;
        } catch (error) {
            logger.log("AddressService :: UpdateAddress :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }
}

const addressService = new Address();
export default addressService