import {Button, Form, Input, Modal} from "antd";
import {useEffect} from "react";
import {useTranslation} from "react-i18next";
import "./SalesforceModal.css";

export interface SalesforceFormValues {
    accountName: string;
    accountPhoneNumber: string;
    accountWebsite: string;
    contactTitle: string;
}

interface Props {
    open: boolean;
    submitting: boolean;
    onCancel: () => void;
    onSubmit: (values: SalesforceFormValues) => void;
}

export default function SalesforceModal({open, submitting, onCancel, onSubmit}: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm<SalesforceFormValues>();

    useEffect(() => {
        if (open) {
            form.resetFields();
        }
    }, [open]);

    const handleSubmit = async () => {
        const values = await form.validateFields();
        onSubmit(values);
    };

    return (
        <Modal
            title={t("salesforce.title")}
            open={open}
            onCancel={onCancel}
            footer={
                <div className="salesforce-modal__footer">
                    <Button
                        onClick={onCancel}
                    >
                        {t("app.cancel")}
                    </Button>
                    <Button
                        type="primary"
                        loading={submitting}
                        onClick={handleSubmit}
                    >
                        {t("salesforce.submit")}
                    </Button>
                </div>
            }
        >
            <Form form={form} layout="vertical">
                <Form.Item
                    label={t("salesforce.accountName")}
                    name="accountName"
                    rules={[{required: true, message: t("salesforce.accountNameRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("salesforce.accountPhoneNumber")}
                    name="accountPhoneNumber"
                    rules={[{required: true, message: t("salesforce.accountPhoneNumberRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("salesforce.accountWebsite")}
                    name="accountWebsite"
                    rules={[{required: true, message: t("salesforce.accountWebsiteRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("salesforce.contactTitle")}
                    name="contactTitle"
                    rules={[{required: true, message: t("salesforce.contactTitleRequired")}]}
                >
                    <Input/>
                </Form.Item>
            </Form>
        </Modal>
    );
}
