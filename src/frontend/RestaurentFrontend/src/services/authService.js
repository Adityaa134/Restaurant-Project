import axiosInstance from "../axios/axiosInstance";

export class AuthService {

    async Login({ userName, password }) {
        try {
            const response = await axiosInstance.post('/Account/login', {
                UserName: userName.toString(),
                Password: password.toString()
            });
            return response.data;
        } catch (error) {
            console.log("AuthService :: Login :: ", error);
            throw new Error(error.response?.data?.detail || error.message || 'Login failed');
        }
    }

    async Logout() {
        try {
            let response = await axiosInstance.get(`/Account/logout`)
        } catch (error) {
            console.log("AuthService :: Logout :: ", error)
        }
    }

    async Register({ email, password, confirmPassword, userName, phoneNumber }) {
        try {
            const response = await axiosInstance.post('/Account/register', {
                Email: email.toString(),
                Password: password.toString(),
                ConfirmPassword: confirmPassword.toString(),
                UserName: userName.toString(),
                PhoneNumber: phoneNumber.toString()
            });
            return response.data;
        } catch (error) {
            console.log("AuthService :: Register :: ", error);
            return false
        }
    }

    async checkEmailExists(email) {
        try {
            const response = await axiosInstance.get(`/Account/EmailExist?email=${email}`);
            return response.data;
        } catch (error) {
            console.error("Check email error:", error);
            return { exists: false };
        }
    }

    async checkUsernameExists(userName) {
        try {
            const response = await axiosInstance.get(`/Account/UserNameExist?username=${userName}`);
            return response.data;
        } catch (error) {
            console.error("Check username error:", error);
            return { exists: false };
        }
    }

    async ResendConfirmEmail(email) {
        if (email == null)
            throw Error("Email can't be null")
        try {
            const response = await axiosInstance.get(`/Account/confirm-email?email=${email}`);
            return response.data;
        } catch (error) {
            return "Email Resend Unsuccessfull"
        }
    }

    async ConfirmEmail(uid, token) {
        if (token == null || uid == null)
            throw Error("Uid or token can't be null")
        try {
            const response = await axiosInstance.post(`/Account/confirm-email-success?uid=${uid}&token=${token}`, {
                uid: uid,
                token: token
            });
            if (!response.ok) {
                throw new Error('Email confirmation failed');
            }
            return response.data;
        } catch (error) {
            return "Email confirmation failed"
        }
    }

    async ForgotPasswordEmail(email) {
        if (email == null || email.trim() === "") {
            throw new Error("Email can't be null or empty");
        }
        try {
            const response = await axiosInstance.get(`/Account/forgot-password?email=${encodeURIComponent(email)}`);
            return {
                success: true,
                message: "Password reset email sent successfully"
            };
        } 
        catch (error) {
            console.log("ForgotPasswordEmail error:", error);
            if (error.response && error.response.status === 400) {
                const backendMessage = error.response.data;
                return {
                    success: false,
                    message: backendMessage 
                };
            }
            return {
                success: false,
                message: "Email sent unsuccessfully"
            };
        }
    }

    async ResetPassword(uid,token,password,confirmPassword){
        try {
            const response = axiosInstance.post(`/Account/reset-password`,{
                Uid : uid.toString(),
                Token:token.toString(),
                Password:password.toString(),
                ConfirmPassword:confirmPassword.toString()
            })
            return true
        } catch (error) {
            console.log("AuthService :: ResetPassword :: ", error);
            return false
        }
    }

    async GoogleLogin(credentials){
       try {
         const response =await axiosInstance.post(`/ExternalLogin/signin-google?credential=${credentials}`)
         return response.data
       } catch (error) {
         console.log("AuthService :: GoogleLogin :: ", error)
       }
    }
}

const authService = new AuthService();
export default authService;