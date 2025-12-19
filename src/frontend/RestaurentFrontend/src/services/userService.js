import axiosInstance from "../axios/axiosInstance"

export class User{
    async GetUserById(userId) {
        try {
            let response = await axiosInstance.get(`/Users/${userId}`)
            return response.data
        }
        catch (error) {
            console.log("User :: GetUserById :: ", error)
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

            const response = await axiosInstance.post(`/users`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            console.log("UserService :: AddProfileImage :: ", error);
            throw error;
        }
    }
}

const userService = new User();
export default userService;