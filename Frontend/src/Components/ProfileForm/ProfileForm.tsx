import {Button, Divider, Form, Input, Select, Upload} from "antd";
import React, {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {UserProfile} from "../../models/UserProfile";
import {UserRole} from "../../models/UserRole";
import ProjectList from "../ProjectList/ProjectList";
import ProfileAttributesTable from "../ProfileAttributeList/ProfileAttributeList";
import CvListTable from "../CvListTable/CvListTable";
import "../../styles/card-wrapper.css";
import "./ProfileForm.css";

interface Props {
    user: UserProfile;
    roles: UserRole[];
    canEditRole: boolean;
    onChange: (user: UserProfile) => void;
    handleSave: (user: UserProfile, redirect: boolean) => Promise<void>;
    handleUploadImage: (id: string, file: File) => Promise<void>;
    onOpenCv: (cvId: string) => void;
}

export default function ProfileForm({
                                        user,
                                        roles,
                                        canEditRole,
                                        handleSave,
                                        onChange,
                                        handleUploadImage,
                                        onOpenCv
                                    }: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm<any>();
    const [imageUrl, setImageUrl] = useState<string>();
    const [fileList, setFileList] = useState<any[]>([]);

    useEffect(() => {
        if (user) {
            form.setFieldsValue({
                ...user,
                roleId: user.role?.id,
            });
            if (user.profileData?.personalPhotoUrl) {
                setImageUrl(user.profileData.personalPhotoUrl);

                setFileList([
                    {
                        name: "avatar",
                        status: "done",
                        url: user.profileData.personalPhotoUrl
                    }
                ]);
            }
        }
    }, [user, form]);

    return (
        <div className="d-flex justify-content-center profile-form__outer">
            <div className="card-wrapper card-wrapper--60">
                <Form
                    form={form}
                    layout="vertical"
                    onFinish={(values) => {
                        const selectedRole = roles.find(role => role.id === values.roleId) || user.role;
                        handleSave({
                            ...user,
                            ...values,
                            profileData: {
                                ...user.profileData,
                                ...values.profileData
                            },
                            role: selectedRole,
                        }, true);
                    }}

                    onValuesChange={() => {
                        const values = form.getFieldsValue(true);
                        const selectedRole = roles.find(role => role.id === values.roleId) || user.role;

                        onChange({
                            ...user,
                            ...values,
                            role: selectedRole,
                        });
                    }}
                >
                    <div className="row g-3">
                        <div className="col-md-9">
                            <div className="row g-3">
                                <div className="col-md-6">
                                    <Form.Item
                                        label={t("profile.firstName")}
                                        name={["profileData", "firstName"]}
                                    >
                                        <Input placeholder={t("profile.firstName")}/>
                                    </Form.Item>
                                </div>
                                <div className="col-md-6">
                                    <Form.Item
                                        label={t("profile.lastName")}
                                        name={["profileData", "lastName"]}
                                    >
                                        <Input placeholder={t("profile.lastName")}/>
                                    </Form.Item>
                                </div>
                                <div className="col-md-6">
                                    <Form.Item
                                        label={t("profile.location")}
                                        name={["profileData", "location"]}
                                    >
                                        <Input placeholder={t("profile.location")}/>
                                    </Form.Item>
                                </div>
                                <div className="col-md-6">
                                    <Form.Item
                                        label={t("profile.email")}
                                        name="email"
                                    >
                                        <Input placeholder={t("profile.email")}/>
                                    </Form.Item>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className=" w-100 h-100 ps-5 pt-5">
                            <Upload
                                listType="picture-card"
                                fileList={fileList}
                                showUploadList={false}
                                className="profile-form__upload"

                                beforeUpload={async (file) => {
                                    const url = URL.createObjectURL(file);

                                    setImageUrl(url);

                                    setFileList([
                                        {
                                            uid: file.uid,
                                            name: file.name,
                                            status: "done",
                                            url: url,
                                        }
                                    ]);

                                    await handleUploadImage(user.id, file);

                                    return false;
                                }}
                            >
                                {
                                    imageUrl ? (
                                        <img
                                            src={imageUrl}
                                            alt={t("profile.profileAlt")}
                                            className="profile-form__avatar"
                                        />
                                    ) : (
                                        <div>
                                            <i className="bi bi-plus-lg"></i>
                                            <div className="mt-2">
                                                {t("profile.upload")}
                                            </div>
                                        </div>
                                    )
                                }
                            </Upload>
                            </div>
                        </div>
                    </div>

                    {canEditRole && (
                        <Form.Item
                            label={t("profile.role")}
                            name="roleId"
                            rules={[{required: true, message: t("profile.roleRequired")}]}
                        >
                            <Select placeholder={t("profile.rolePlaceholder")}>
                                {roles.map(role => (
                                    <Select.Option key={role.id} value={role.id}>
                                        {role.name}
                                    </Select.Option>
                                ))}
                            </Select>
                        </Form.Item>
                    )}

                    <Divider orientation="horizontal">
                        {t("profile.projects")}
                    </Divider>

                    <ProjectList/>

                    <Divider className="mb-4"/>

                    <Divider orientation="horizontal" className="mt-4">
                        {t("profile.attributes")}
                    </Divider>

                    <ProfileAttributesTable/>
                    <Divider className="mb-4"/>

                    <Divider orientation="horizontal" className="mt-4">
                        {t("profile.cvs")}
                    </Divider>
                    <div className="my-3">
                        <CvListTable data={user.cvs} handleOpen={(cv) => onOpenCv(cv.positionId)}/>
                    </div>

                    <Divider className="my-4"/>

                    <div className="d-flex justify-content-end">
                        <Button
                            type="primary"
                            htmlType="submit"
                            className="d-flex align-items-center gap-2"
                        >
                            <i className="bi bi-floppy"></i>
                            <span>{t("app.saveChanges")}</span>
                        </Button>
                    </div>
                </Form>
            </div>
        </div>
    );
}
