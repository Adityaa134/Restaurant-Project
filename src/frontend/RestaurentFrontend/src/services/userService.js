import axiosInstance from "../axios/axiosInstance"

export class User{
    async GetUserById(userId) {
        try {
            let response = await axiosInstance.get(`/api/Users/${userId}`)
            return response.data
        }
        catch (error) {
            logger.log("UserService :: GetUserById :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }

    }

    async AddProfileImage({ userId ,userName, profileImagePath, phonenumber, profileImage }) {
        try {
            const formData = new FormData();
            formData.append('UserId', userId.toString());
            formData.append('ProfileImage', profileImage[0]);
            formData.append('ProfileImagePath', profileImagePath);
            formData.append('UserName', userName);
            formData.append('Phonenumber', phonenumber.toString());

            const response = await axiosInstance.post(`/api/users`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            logger.log("UserService :: AddProfileImage :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            throw error;
        }
    }
}

const userService = new User();
export default userService;