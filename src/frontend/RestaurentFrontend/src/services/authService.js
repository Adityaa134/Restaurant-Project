import axiosInstance from "../axios/axiosInstance";
import { logger } from "../utils/logger";

export class AuthService {

    async Login({ userName, password }) {
        try {
            const response = await axiosInstance.post('/api/Account/login', {
                UserName: userName.toString(),
                Password: password.toString()
            });
            return response.data;
        } catch (error) {
            logger.error("AuthService :: Login :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });

            if(error.response?.status == 401){
                throw new Error("Invalid Username or password");
            }
            else{
                throw new Error(error.response?.data?.detail || error.message || "Login Failed");
            }
        }
    }

    async Logout() {
        try {
            let response = await axiosInstance.get(`/api/Account/logout`)
        } catch (error) {
             logger.error("AuthService :: Logout :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }

    async Register({ email, password, confirmPassword, userName, phoneNumber }) {
        try {
            const response = await axiosInstance.post('/api/Account/register', {
                Email: email.toString(),
                Password: password.toString(),
                ConfirmPassword: confirmPassword.toString(),
                UserName: userName.toString(),
                PhoneNumber: phoneNumber.toString()
            });
            return response.data;
        } catch (error) {
            logger.error("AuthService :: Register :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async checkEmailExists(email) {
        try {
            const response = await axiosInstance.get(`/api/Account/EmailExist?email=${email}`);
            return response.data;
        } catch (error) {
            logger.log("AuthService :: checkEmailExists :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return { exists: false };
        }
    }

    async checkUsernameExists(userName) {
        try {
            const response = await axiosInstance.get(`/api/Account/UserNameExist?username=${userName}`);
            return response.data;
        } catch (error) {
            logger.log("AuthService :: checkUsernameExists :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return { exists: false };
        }
    }

    async ResendConfirmEmail(email) {
        if (email == null)
            throw Error("Email can't be null")
        try {
            const response = await axiosInstance.get(`/api/Account/confirm-email?email=${email}`);
            return response.data;
        } catch (error) {
            logger.log("AuthService :: ResendConfirmEmail :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return "Email Resend Unsuccessfull"
        }
    }

    async ConfirmEmail(uid, token) {
        if (token == null || uid == null)
            throw Error("Uid or token can't be null")
        try {
            const response = await axiosInstance.post(`/api/Account/confirm-email-success?uid=${uid}&token=${token}`, {
                uid: uid,
                token: token
            });
            if (!response.ok) {
                throw new Error('Email confirmation failed');
            }
            return response.data;
        } catch (error) {
            logger.error("AuthService :: ConfirmEmail :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return "Email confirmation failed"
        }
    }

    async ForgotPasswordEmail(email) {
        if (email == null || email.trim() === "") {
            throw new Error("Email can't be null or empty");
        }
        try {
            const response = await axiosInstance.get(`/api/Account/forgot-password?email=${encodeURIComponent(email)}`);
            return {
                success: true,
                message: "Password reset email sent successfully"
            };
        } 
        catch (error) {
            logger.error("AuthService :: ForgotPasswordEmail :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });

            if (error.response){
                const backendMessage = error.response.data?.detail || error.response.data || "Something went wrong";
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
            const response = axiosInstance.post(`/api/Account/reset-password`,{
                Uid : uid.toString(),
                Token:token.toString(),
                Password:password.toString(),
                ConfirmPassword:confirmPassword.toString()
            })
            return true
        } catch (error) {
            logger.error("AuthService :: ResetPassword :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async GoogleLogin(credentials){
       try {
         const response =await axiosInstance.post(`/api/ExternalLogin/signin-google?credential=${credentials}`)
         return response.data
       } catch (error) {
         logger.error("AuthService :: GoogleLogin :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
       }
    }

    async RestoreSession(){
        try {
            const response = await axiosInstance.get("/api/Account/restore-session")
            return response.data;
        } catch (error) {
            logger.error("AuthService :: RestoreSession :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }
}

const authService = new AuthService();
export default authService;