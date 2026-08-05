import {Button, Form, Input, Modal, Select} from "antd";
import {useEffect} from "react";
import {useTranslation} from "react-i18next";
import {Priority} from "../../models/Priority";

export interface SupportTicketFormValues {
    summary: string;
    priority: Priority;
}

interface Props {
    open: boolean;
    submitting: boolean;
    onCancel: () => void;
    onSubmit: (values: SupportTicketFormValues) => void;
}

export default function SupportTicketModal({open, submitting, onCancel, onSubmit}: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm<SupportTicketFormValues>();

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
            title={t("supportTicket.title")}
            open={open}
            onCancel={onCancel}
            footer={
                <div className="d-flex justify-content-end gap-2">
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
                        {t("supportTicket.send")}
                    </Button>
                </div>
            }
        >
            <Form form={form} layout="vertical">
                <Form.Item
                    label={t("supportTicket.priorityLabel")}
                    name="priority"
                    rules={[{required: true, message: t("supportTicket.priorityRequired")}]}
                >
                    <Select
                        placeholder={t("supportTicket.priorityPlaceholder")}
                        options={[
                            {value: Priority.Low, label: t("supportTicket.priorityLow")},
                            {value: Priority.Medium, label: t("supportTicket.priorityMedium")},
                            {value: Priority.High, label: t("supportTicket.priorityHigh")},
                        ]}
                    />
                </Form.Item>
                <Form.Item
                    label={t("supportTicket.summaryLabel")}
                    name="summary"
                    rules={[{required: true, message: t("supportTicket.summaryRequired")}]}
                >
                    <Input.TextArea
                        rows={4}
                    />
                </Form.Item>
            </Form>
        </Modal>
    );
}
