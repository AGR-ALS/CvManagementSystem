import {message} from "antd";
import {useTranslation} from "react-i18next";
import AuthenticationForm from "../Components/AuthenticationForm/AuthenticationForm";
import {useNavigate, useSearchParams} from "react-router";
import {Login, Register} from "../Services/AuthService";
import {SendVerificationEmail} from "../Services/MailService";
import {LoginUserRequest} from "../models/LoginUserRequest";
import {AuthMode} from "../models/AuthMode";
import {useAuth} from "../Contexts/AuthContext";

export default function AuthenticationPage() {
    const navigate = useNavigate();
    const {t} = useTranslation();
    const [searchParams] = useSearchParams();
    const auth = useAuth();

    const isRegister = searchParams.get('isRegister') === 'true';

    const initialMode = isRegister ? AuthMode.Register : AuthMode.Login;

    const handleSubmit = async (email: string, password: string, mode: AuthMode, rememberMe: boolean) => {
        try {
            if (mode === AuthMode.Register) {
                await Register({email, password, rememberMe} as LoginUserRequest);
                await Login({email, password, rememberMe} as LoginUserRequest);
                await auth.login();
                await SendVerificationEmail(email);
                message.info(t("auth.verificationEmailSent"));
                navigate("/account-confirm");
            } else {
                await Login({email, password, rememberMe} as LoginUserRequest);
                await auth.login();
                navigate("/profile?mode=view");
            }
        } catch {
            message.error(t("auth.loginError"));
        }
    }

    return (
        <AuthenticationForm
            initialMode={initialMode}
            onSubmit={handleSubmit}
        />
    );
}