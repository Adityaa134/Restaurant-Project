import axiosInstance from "../axios/axiosInstance"

export class Address {

    async GetAddressesOfUser(userId) {
        try {
            let response = await axiosInstance.get(`/Address/user-address/${userId}`)
            return response.data
        }
        catch (error) {
            console.log("AddressService :: GetAddressesOfUser :: ", error)
            return false
        }
    }

    async GetAddressById(addressId) {
        try {
            let response = await axiosInstance.get(`/Address/${addressId}`)
            return response.data
        } catch (error) {
            console.log("AddressService :: GetAddressById :: ", error)
            return false
        }
    }

    async CreateAddress({userId, addressLine, city, landmark, area}) {
        try {
            const response = await axiosInstance.post(`/Address`,
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
            console.log("AddressService :: CreateAddress :: ", error);
        }
    }

    async UpdateAddress({addressId, userId, addressLine, city, landmark, area}) {
        try {
            const response = await axiosInstance.put(`/Address`,{
                    AddressId:addressId,   
                    UserId:userId,
                    AddressLine:addressLine,
                    City:city,
                    Landmark:landmark,
                    Area:area
                });

            return response.data;
        } catch (error) {
            console.log("AddressService :: UpdateAddress :: ", error);
        }
    }
}

const addressService = new Address();
export default addressService