import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

export default function Protected({
    children,
    authentication = true,
    requiredRole = null
}) {
    const navigate = useNavigate();
    const { authStatus, role, authChecked } = useSelector(
        state => state.auth
    );

    useEffect(() => {
        if (!authChecked) return;

        if (authentication && !authStatus) {
            navigate("/login", { replace: true }); // replace history to avoid back navigation if not given then user can be in redirect loop if he press back button 
            return;
        }

        if (!authentication && authStatus) {
            navigate("/", { replace: true });
            return;
        }

        if (authentication && authStatus && requiredRole) {
            if (!role) return;
            if (role !== requiredRole) {
                navigate("/page-not-exist", { replace: true });
                return;
            }
        }
    }, [authChecked, authStatus, role, navigate, authentication, requiredRole]);

    if (!authChecked) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="w-10 h-10 border-4 border-gray-300 border-t-black rounded-full animate-spin"></div>
            </div>
        );
    }

    return <>{children}</>;
}