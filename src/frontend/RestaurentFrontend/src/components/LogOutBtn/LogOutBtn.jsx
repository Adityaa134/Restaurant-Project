import { useDispatch } from 'react-redux'
import authService from '../../services/authService'
import { logout } from '../../features/auth/authSlice'

function LogOutBtn() {
    const dispatch = useDispatch()

    const logoutHandler = () => {
        authService.Logout()
            .then(() => {
                dispatch(logout())
            })
        localStorage.removeItem("token")
        localStorage.removeItem("refreshToken")
    }

    return (
        <button
            className="text-gray-600 hover:text-blue-600 font-medium text-sm transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-gray-50"
            onClick={logoutHandler}
        >
            Logout
        </button>
    )
}

export default LogOutBtn