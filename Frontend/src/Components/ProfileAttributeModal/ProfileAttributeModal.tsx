import {Modal} from "antd";
import {AttributeValue} from "../../models/AttributeValue";
import {ImageUrlResponse} from "../../models/ImageUrl";
import {useEffect, useState} from "react";
import ReactMarkdown from "react-markdown";
import "./ProfileAttributeModal.css";

interface Props {
    open: boolean;
    attribute: AttributeValue | null;
    onClose: () => void;
    loadImage: (key: string) => Promise<ImageUrlResponse>;
}

export default function AttributeValueModal({
                                                open,
                                                attribute,
                                                onClose,
                                                loadImage
                                            }: Props) {
    if (!attribute) {
        return null;
    }

    const [imageUrl, setImageUrl] = useState<ImageUrlResponse | undefined>(undefined);
    useEffect(() => {
        loadImage(String(attribute.value)).then(setImageUrl);
    }, [attribute]);

    const renderValue = () => {
        switch (attribute.attributeDefinition.dataType) {
            case 1:
                return (
                    <div className="profile-attr-modal__markdown">
                        <ReactMarkdown>
                            {String(attribute.value)}
                        </ReactMarkdown>
                    </div>
                );

            case 2:
                return (
                    <img
                        src={imageUrl?.url}
                        alt={attribute.attributeDefinition.name}
                        className="profile-attr-modal__image"
                    />
                );

            default:
                return <div>{String(attribute.value)}</div>;
        }
    };

    return (
        <Modal
            open={open}
            title={attribute.attributeDefinition.name}
            onCancel={onClose}
            footer={null}
            width={700}
        >
            {renderValue()}
        </Modal>
    );
}