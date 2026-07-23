import {useState} from "react";
import {Button, Checkbox, Form, FormProps, Input} from "antd";
import {useTranslation} from "react-i18next";
import {AuthMode} from "../../models/AuthMode";
import "./AuthenticationForm.css";

type FieldType = {
    email?: string;
    password?: string;
    rememberMe?: boolean;
};

interface Props {
    initialMode: AuthMode;
    onSubmit: (email: string, password: string, mode: AuthMode, rememberMe: boolean) => void;
}

export default function AuthenticationForm({initialMode, onSubmit}: Props) {
    const {t} = useTranslation();
    const [mode, setMode] = useState<AuthMode>(initialMode);
    const onFinish: FormProps<FieldType>['onFinish'] = (values) => {
        onSubmit(values.email || "", values.password || "", mode, values.rememberMe ?? false);
    };

    const toggleMode = () => {
        setMode(mode === AuthMode.Login ? AuthMode.Register : AuthMode.Login);
    };

    return (
        <div className="d-flex justify-content-center align-items-center bg-light auth-form__outer">
            <div className="card shadow-sm p-4 auth-form__card">
                <h3 className="text-center mb-4">
                    {mode === AuthMode.Login ? t("auth.loginTitle") : t("auth.registerTitle")}
                </h3>

                <Form
                    name="auth_form"
                    layout="vertical"
                    initialValues={{rememberMe: true}}
                    onFinish={onFinish}
                    autoComplete="off"
                >
                    {/* 2. Поле Email с валидацией */}
                    <Form.Item<FieldType>
                        label={t("auth.email")}
                        name="email"
                        rules={[
                            {required: true, message: t("auth.emailRequired")},
                            {type: 'email', message: t("auth.emailInvalid")}
                        ]}
                    >
                        <Input placeholder={t("auth.emailPlaceholder")}/>
                    </Form.Item>

                    <Form.Item<FieldType>
                        label={t("auth.password")}
                        name="password"
                        rules={[{required: true, message: t("auth.passwordRequired")}]}
                    >
                        <Input.Password placeholder={t("auth.passwordPlaceholder")}/>
                    </Form.Item>

                    <Form.Item<FieldType> name="rememberMe" valuePropName="checked">
                        <Checkbox>{t("auth.rememberMe")}</Checkbox>
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" className="w-100">
                            {mode === AuthMode.Login ? t("auth.signIn") : t("auth.signUp")}
                        </Button>
                    </Form.Item>

                    <div className="text-center mt-2">
                        <small className="text-muted">
                            {mode === AuthMode.Login ? (
                                <span
                                    className="text-secondary auth-form__toggle"
                                    onClick={toggleMode}
                                >
                                        {t("auth.noAccount")}
                                    </span>
                            ) : (
                                <span
                                    className="text-secondary auth-form__toggle"
                                    onClick={toggleMode}
                                >
                                    {t("auth.hasAccount")}
                                </span>
                            )}
                        </small>
                    </div>
                </Form>

                <div className="d-flex gap-2 mt-3">
                    <Button
                        type="default"
                        className="w-100"
                        onClick={() => window.location.href = `${process.env.REACT_APP_API_URL}/Users/google-login`}
                    >
                        {t("auth.google")}
                    </Button>
                    <Button
                        type="default"
                        className="w-100"
                        onClick={() => window.location.href = `${process.env.REACT_APP_API_URL}/Users/facebook-login`}
                    >
                        {t("auth.facebook")}
                    </Button>
                </div>
            </div>
        </div>
    );
}