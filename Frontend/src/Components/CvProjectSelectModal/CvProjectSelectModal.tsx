import {useEffect, useState} from "react";
import {Button, Form, Modal, Select} from "antd";
import {useTranslation} from "react-i18next";
import {Project} from "../../models/Project";

interface Props {
    open: boolean;
    projects: Project[];
    selectedProjectIds: string[];
    maxSelectable: number;
    onClose: () => void;
    onSave: (selectedProjectIds: string[]) => void;
}

export default function CvProjectSelectModal({
                                                 open,
                                                 projects,
                                                 selectedProjectIds,
                                                 maxSelectable,
                                                 onClose,
                                                 onSave,
                                             }: Props) {
    const {t} = useTranslation();
    const [selectedId, setSelectedId] = useState<string | undefined>(undefined);

    useEffect(() => {
        if (open) {
            setSelectedId(undefined);
        }
    }, [open]);

    const availableOptions = projects
        .filter((project) => !selectedProjectIds.includes(project.id))
        .map((project) => ({
            value: project.id,
            label: project.name,
        }));

    const handleSave = () => {
        if (selectedId) {
            onSave([selectedId]);
        }
        onClose();
    };

    return (
        <Modal
            title={t("cvProjectSelect.title")}
            open={open}
            onCancel={onClose}
            footer={[
                <Button key="cancel" onClick={onClose}>
                    {t("app.cancel")}
                </Button>,
                <Button key="save" type="primary" onClick={handleSave} disabled={!selectedId}>
                    {t("app.add")}
                </Button>,
            ]}
        >
            <Form layout="vertical">
                <Form.Item label={t("cvProjectSelect.project")}>
                    <Select
                        showSearch={{
                            filterOption: (input, option) =>
                                (option?.label ?? "").toLowerCase().includes(input.toLowerCase()),
                        }}
                        placeholder={t("cvProjectSelect.selectProject")}
                        value={selectedId}
                        onChange={setSelectedId}
                        options={availableOptions}
                    />
                </Form.Item>
            </Form>
        </Modal>
    );
}
