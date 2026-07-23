import {useEffect, useState} from "react";
import {Button, DatePicker, Form, Input, InputNumber, Modal, Select, Switch} from "antd";
import {useTranslation} from "react-i18next";
import {AttributeDefinition} from "../../models/AttributeDefinition";
import {AttributeDataType} from "../../models/AttributeDataType";
import {AttributeValue} from "../../models/AttributeValue";
import {FilterOperator} from "../../models/FilterOperator";
import dayjs from "dayjs";
import {usePositionFormContext} from "../../Contexts/PositionFormContext";

interface Props {
    open: boolean;
    onClose: () => void;
    onSave: (rule: { filterOperator: FilterOperator; attributeValue: AttributeValue }) => void;
    initialRule?: { filterOperator: FilterOperator; attributeValue: AttributeValue } | null;
}

const getFilterOperators = (dataType: AttributeDataType): FilterOperator[] => {
    switch (dataType) {
        case AttributeDataType.String:
        case AttributeDataType.Text:
            return [FilterOperator.Contains, FilterOperator.NotContains];
        case AttributeDataType.Image:
            return [FilterOperator.Has];
        case AttributeDataType.Numeric:
        case AttributeDataType.Date:
            return [
                FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.GreaterThan,
                FilterOperator.LessThan, FilterOperator.GreaterThanOrEqual, FilterOperator.LessThanOrEqual
            ];
        case AttributeDataType.Period:
            return [FilterOperator.Intersects];
        case AttributeDataType.Boolean:
        case AttributeDataType.OneOfMany:
            return [FilterOperator.Equal, FilterOperator.NotEqual];
        default:
            return [];
    }
};

const dataTypeLabel = (type: AttributeDataType, t: (key: string) => string) =>
    [t("dataTypes.string"), t("dataTypes.text"), t("dataTypes.image"), t("dataTypes.numeric"), t("dataTypes.date"), t("dataTypes.period"), t("dataTypes.boolean"), t("dataTypes.oneOfMany")][type] ?? type;

const filterOperatorLabel = (op: FilterOperator, t: (key: string) => string): string =>
    [t("operators.equal"), t("operators.notEqual"), t("operators.greaterThan"), t("operators.lessThan"), t("operators.greaterThanOrEqual"), t("operators.lessThanOrEqual"), t("operators.contains"), t("operators.notContains"), t("operators.has"), t("operators.intersects")][op] ?? String(op);

export default function PositionAccessRuleEditModal({open, onClose, onSave, initialRule}: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm();
    const {attributeDefinitions: definitions} = usePositionFormContext();
    const [selectedDefinition, setSelectedDefinition] = useState<AttributeDefinition | null>(null);

    useEffect(() => {
        if (open) {
            if (initialRule) {
                
                let formValue: any = initialRule.attributeValue.value;
                const dataType = initialRule.attributeValue.attributeDefinition.dataType;

                
                if (typeof formValue === 'object' && formValue !== null && 'oneOfManyValueId' in formValue) {
                    formValue = (formValue as any).oneOfManyValueId;
                }
                
                else if (dataType === AttributeDataType.Date && typeof formValue === 'string') {
                    formValue = dayjs(formValue);
                }
                
                else if (dataType === AttributeDataType.Period && typeof formValue === 'object' && formValue !== null && 'start' in formValue) {
                    formValue = [
                        dayjs((formValue as any).start),
                        dayjs((formValue as any).end)
                    ];
                }

                form.setFieldsValue({
                    filterOperator: initialRule.filterOperator,
                    attributeDefinitionId: initialRule.attributeValue.attributeDefinition.id,
                    value: formValue
                });
                setSelectedDefinition(initialRule.attributeValue.attributeDefinition);
            } else {
                form.resetFields();
                setSelectedDefinition(null);
            }
        }
    }, [open, initialRule, form]);

    const handleDefinitionChange = (id: string) => {
        const def = definitions.find(d => d.id === id) || null;
        setSelectedDefinition(def);
        form.setFieldsValue({value: undefined, filterOperator: undefined});
    };

    const renderValueField = () => {
        if (!selectedDefinition) return null;

        switch (selectedDefinition.dataType) {
            case AttributeDataType.String:
            case AttributeDataType.Text:
                return (
                    <Form.Item label={t("accessRuleEdit.value")} name="value" rules={[{required: true, message: t("accessRuleEdit.valueRequired")}]}>
                        {selectedDefinition.dataType === AttributeDataType.Text ? <Input.TextArea rows={6}/> : <Input/>}
                    </Form.Item>
                );
            case AttributeDataType.Numeric:
                return (
                    <Form.Item label={t("accessRuleEdit.value")} name="value" rules={[{required: true, message: t("accessRuleEdit.valueRequired")}]}>
                        <InputNumber className="w-100"/>
                    </Form.Item>
                );
            case AttributeDataType.Date:
                return (
                    <Form.Item label={t("accessRuleEdit.value")} name="value" rules={[{required: true, message: t("accessRuleEdit.dateRequired")}]}>
                        <DatePicker className="w-100"/>
                    </Form.Item>
                );
            case AttributeDataType.Period:
                return (
                    <Form.Item label={t("accessRuleEdit.period")} name="value" rules={[{required: true, message: t("accessRuleEdit.periodRequired")}]}>
                        <DatePicker.RangePicker className="w-100"/>
                    </Form.Item>
                );
            case AttributeDataType.Boolean:
                return (
                    <Form.Item label={t("accessRuleEdit.value")} name="value" initialValue={false}>
                        <Switch/>
                    </Form.Item>
                );
            case AttributeDataType.OneOfMany:
                return (
                    <Form.Item label={t("accessRuleEdit.value")} name="value" rules={[{required: true, message: t("accessRuleEdit.selectValue")}]}>
                        <Select>
                            {selectedDefinition.oneOfManyOptions?.map(opt => (
                                <Select.Option key={opt.id} value={opt.id}>{opt.value}</Select.Option>
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

        let finalValue = values.value;

        
        if (selectedDefinition.dataType === AttributeDataType.OneOfMany) {
            const selectedOption = selectedDefinition.oneOfManyOptions?.find(opt => opt.id === values.value);
            finalValue = {
                oneOfManyValueId: values.value,
                value: selectedOption ? selectedOption.value : values.value
            };
        } else if (selectedDefinition.dataType === AttributeDataType.Date && dayjs.isDayjs(values.value)) {
            finalValue = values.value.format('YYYY-MM-DD');
        } else if (selectedDefinition.dataType === AttributeDataType.Period && Array.isArray(values.value)) {
            finalValue = {
                start: values.value[0] ? dayjs(values.value[0]).format('YYYY-MM-DD') : null,
                end: values.value[1] ? dayjs(values.value[1]).format('YYYY-MM-DD') : null
            };
        }

        onSave({
            filterOperator: values.filterOperator,
            attributeValue: {
                id: initialRule?.attributeValue.id || "",
                attributeDefinition: selectedDefinition,
                value: finalValue
            }
        });
    };

    return (
        <Modal
            title={initialRule ? t("accessRuleEdit.editTitle") : t("accessRuleEdit.addTitle")}
            open={open}
            onCancel={onClose}
            footer={[
                <Button key="cancel" onClick={onClose}>{t("app.cancel")}</Button>,
                <Button key="save" type="primary" onClick={handleOk}>{t("app.save")}</Button>,
            ]}
        >
            <Form form={form} layout="vertical">
                <Form.Item label={t("accessRuleEdit.attributeDefinition")} required>
                    <Select onChange={handleDefinitionChange} placeholder={t("accessRuleEdit.selectAttributeDefinition")}
                            value={selectedDefinition?.id}>
                        {definitions.map(def => (
                            <Select.Option key={def.id} value={def.id}>
                                {def.name} ({dataTypeLabel(def.dataType, t)})
                            </Select.Option>
                        ))}
                    </Select>
                </Form.Item>

                {selectedDefinition && (
                    <Form.Item label={t("accessRuleEdit.filterOperator")} name="filterOperator"
                               rules={[{required: true, message: t("accessRuleEdit.filterOperatorRequired")}]}>
                        <Select>
                            {getFilterOperators(selectedDefinition.dataType).map(op => (
                                <Select.Option key={op} value={op}>{filterOperatorLabel(op, t)}</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                )}

                {renderValueField()}
            </Form>
        </Modal>
    );
}
