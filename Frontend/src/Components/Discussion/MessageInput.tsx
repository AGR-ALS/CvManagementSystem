import {useState} from "react";
import {useTranslation} from "react-i18next";
import {Button, Input} from "antd";
import {SendOutlined} from "@ant-design/icons";

interface Props {
    onSend: (text: string) => Promise<void>;
    disabled?: boolean;
}

export default function MessageInput({onSend, disabled}: Props) {
    const {t} = useTranslation();
    const [text, setText] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSend = async () => {
        if (!text.trim() || disabled) return;
        setLoading(true);
        try {
            await onSend(text);
            setText("");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="card shadow-sm border">
            <div className="card-body">
                <Input.TextArea
                    rows={5}
                    placeholder={t("discussion.placeholder")}
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    disabled={disabled || loading}
                    className="mb-2 resize-none"
                    onKeyDown={(e) => {
                        if (e.key === "Enter" && !e.shiftKey) {
                            e.preventDefault();
                            handleSend();
                        }
                    }}
                />
                <div className="d-flex justify-content-end">
                    <Button
                        type="primary"
                        icon={<SendOutlined/>}
                        onClick={handleSend}
                        loading={loading}
                        disabled={!text.trim() || disabled}
                    >
                        {t("discussion.send")}
                    </Button>
                </div>
            </div>
        </div>
    );
}