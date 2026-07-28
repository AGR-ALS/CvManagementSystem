import {useEffect, useState} from "react";
import dayjs from "dayjs";
import {Button, DatePicker, Form, Input, InputNumber, Modal, Select, Switch, Upload} from "antd";
import {UploadOutlined} from "@ant-design/icons";
import {useTranslation} from "react-i18next";
import {AttributeDefinition} from "../../models/AttributeDefinition";
import {AttributeDataType} from "../../models/AttributeDataType";
import {CreateUpdateAttributeValueRequest} from "../../models/CreateUpdateAttributeValueRequest";

interface Props {
    open: boolean;
    definitions: AttributeDefinition[];
    onClose: () => void;
    onSave: (type: AttributeDataType, data: CreateUpdateAttributeValueRequest) => Promise<void>;
    isEditing?: boolean;
    selectedDefinition?: AttributeDefinition | null;
    initialValue?: any;
}

const dataTypeLabel = (type: number, t: (key: string) => string) =>
    [t("dataTypes.string"), t("dataTypes.text"), t("dataTypes.image"), t("dataTypes.numeric"), t("dataTypes.date"), t("dataTypes.period"), t("dataTypes.boolean"), t("dataTypes.oneOfMany")][type] ?? type;

export default function ProfileAttributeEditModal({
                                                      open,
                                                      definitions,
                                                      onClose,
                                                      onSave,
                                                      isEditing = false,
                                                      selectedDefinition: externalDefinition = null,
                                                      initialValue,
                                                  }: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm();
    const [selectedDefinition, setSelectedDefinition] = useState<AttributeDefinition | null>(externalDefinition);
    const [imageFile, setImageFile] = useState<File | null>(null);

    const prepareInitialValue = (value: any) => {
        if (!externalDefinition) {
            return value;
        }

        switch (externalDefinition.dataType) {
            case AttributeDataType.Date:
                return value ? dayjs(value) : undefined;
            case AttributeDataType.Period:
                return value ? [dayjs(value.start), dayjs(value.end)] : undefined;
            default:
                return value;
        }
    };

    useEffect(() => {
        if (open) {
            form.resetFields();
            setSelectedDefinition(externalDefinition ?? null);
            setImageFile(null);
            if (externalDefinition && initialValue !== undefined) {
                form.setFieldsValue({value: prepareInitialValue(initialValue)});
            }
        }
    }, [open, externalDefinition, initialValue]);

    const handleDefinitionChange = (id: string) => {
        const def = definitions.find(d => d.id === id) || null;
        setSelectedDefinition(def);
        form.setFieldsValue({value: undefined});
    };

    const renderValueField = () => {
        if (!selectedDefinition) return null;

        switch (selectedDefinition.dataType) {
            case AttributeDataType.String:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" rules={[{required: true, message: t("profileAttributeEdit.valueRequired")}]}>
                        <Input/>
                    </Form.Item>
                );

            case AttributeDataType.Text:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" rules={[{required: true, message: t("profileAttributeEdit.valueRequired")}]}>
                        <Input.TextArea rows={6}/>
                    </Form.Item>
                );

            case AttributeDataType.Image:
                return (
                    <Form.Item label={t("profileAttributeEdit.image")} required>
                        <Upload
                            beforeUpload={(file) => {
                                setImageFile(file);
                                return false;
                            }}
                            maxCount={1}
                            onRemove={() => setImageFile(null)}
                            fileList={imageFile ? [{uid: '-1', name: imageFile.name, status: 'done'}] : []}
                        >
                            <Button icon={<UploadOutlined/>}>{t("profileAttributeEdit.selectImage")}</Button>
                        </Upload>
                    </Form.Item>
                );

            case AttributeDataType.Numeric:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" rules={[{required: true, message: t("profileAttributeEdit.valueRequired")}]}>
                        <InputNumber className="w-100"/>
                    </Form.Item>
                );

            case AttributeDataType.Date:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" rules={[{required: true, message: t("profileAttributeEdit.dateRequired")}]}>
                        <DatePicker className="w-100"/>
                    </Form.Item>
                );

            case AttributeDataType.Period:
                return (
                    <Form.Item label={t("profileAttributeEdit.period")} name="value" rules={[{required: true, message: t("profileAttributeEdit.periodRequired")}]}>
                        <DatePicker.RangePicker className="w-100"/>
                    </Form.Item>
                );

            case AttributeDataType.Boolean:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" initialValue={false}>
                        <Switch/>
                    </Form.Item>
                );

            case AttributeDataType.OneOfMany:
                return (
                    <Form.Item label={t("profileAttributeEdit.value")} name="value" rules={[{required: true, message: t("profileAttributeEdit.selectValue")}]}>
                        <Select>
                            {selectedDefinition.oneOfManyOptions?.map(opt => (
                                <Select.Option key={opt.id} value={opt.id}>
                                    {opt.value}
                                </Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                );

            default:
                return null;
        }
    };

    const handleOk = async () => {
        const values = await form.validateFields();
        if (!selectedDefinition) return;

        const dataType = selectedDefinition.dataType;

        const data: CreateUpdateAttributeValueRequest = {
            attributeDefinitionId: selectedDefinition.id,
        };

        if (dataType === AttributeDataType.String) {
            data.stringValue = values.value;
        } else if (dataType === AttributeDataType.Text) {
            data.markDownValue = values.value;
        } else if (dataType === AttributeDataType.Image) {
            if (!imageFile) {
                return;
            }
            data.imageValue = imageFile;
        } else if (dataType === AttributeDataType.Numeric) {
            data.numericValue = values.value;
        } else if (dataType === AttributeDataType.Date) {
            data.dateValue = values.value?.format('YYYY-MM-DD');
        } else if (dataType === AttributeDataType.Period) {
            if (values.value) {
                data.periodStartValue = values.value[0]?.format('YYYY-MM-DD');
                data.periodEndValue = values.value[1]?.format('YYYY-MM-DD');
            }
        } else if (dataType === AttributeDataType.Boolean) {
            data.booleanValue = values.value;
        } else if (dataType === AttributeDataType.OneOfMany) {
            data.oneOfManyValueId = values.value;
        }

        await onSave(selectedDefinition.dataType, data);
    };

    const renderDefinitionField = () => {
        if (isEditing && selectedDefinition) {
            return (
                <Form.Item label={t("profileAttributeEdit.attributeDefinition")}>
                    <Input value={`${selectedDefinition.name} (${dataTypeLabel(selectedDefinition.dataType, t)})`}
                           disabled/>
                </Form.Item>
            );
        }

        return (
            <Form.Item label={t("profileAttributeEdit.attributeDefinition")} required>
                <Select onChange={handleDefinitionChange} placeholder={t("profileAttributeEdit.selectAttributeDefinition")}
                        value={selectedDefinition?.id}>
                    {definitions.map(def => (
                        <Select.Option key={def.id} value={def.id}>
                            {def.name} ({dataTypeLabel(def.dataType, t)})
                        </Select.Option>
                    ))}
                </Select>
            </Form.Item>
        );
    };

    return (
        <Modal
            title={isEditing ? t("profileAttributeEdit.editTitle") : t("profileAttributeEdit.addTitle")}
            open={open}
            onCancel={onClose}
            footer={[
                <Button key="cancel" onClick={onClose}>
                    {t("app.cancel")}
                </Button>,
                <Button key="save" type="primary" onClick={handleOk}>
                    {t("app.save")}
                </Button>,
            ]}
        >
            <Form form={form} layout="vertical">
                {renderDefinitionField()}

                {renderValueField()}
            </Form>
        </Modal>
    );
}
