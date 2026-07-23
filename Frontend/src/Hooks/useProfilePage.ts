import {useEffect, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {NavigateFunction} from "react-router";
import {UserProfile} from "../models/UserProfile";
import {UserRole} from "../models/UserRole";
import {
    GetRoles,
    GetUserBasicInfo,
    GetUserImage,
    UpdateUser,
    UploadUserImage
} from "../Services/UserService";
import {GetUserProjects} from "../Services/ProjectService";
import {GetAttributeImage, GetUserAttributes} from "../Services/AttributeService";
import {GetUserCvs} from "../Services/CvService";
import {SearchTechnologies} from "../Services/TechnologiesService";
import {useAuth} from "../Contexts/AuthContext";

const defaultUser: UserProfile = {
    id: "",
    profileData: undefined,
    role: {id: "", name: ""},
    projects: [],
    version: 0,
    email: "",
    attributeValues: [],
    cvs: [],
    isBlocked: false
};

export function useProfilePage(searchParams: URLSearchParams, navigate: NavigateFunction) {
    const {t} = useTranslation();
    const auth = useAuth();
    const [user, setUser] = useState<UserProfile>(defaultUser);
    const [roles, setRoles] = useState<{ id: string; name: string }[]>([]);
    const [currentUserId, setCurrentUserId] = useState("");
    const [currentUserRole, setCurrentUserRole] = useState<UserRole | null>(null);
    const [loading, setLoading] = useState(true);
    const [technologyOptions, setTechnologyOptions] = useState<{ value: string; label: string }[]>([]);
    const isViewMode = searchParams.get("mode") !== "edit";
    const profileUserId = searchParams.get("userId");

    useEffect(() => {
        const load = async () => {
            const id = profileUserId || auth.userId;
            if (!id) return;
            setLoading(true);
            try {
                setCurrentUserId(auth.userId);
                setCurrentUserRole(auth.role);
                const [profileUser, userRoles, projects, attributes, cvs] = await Promise.all([GetUserBasicInfo(id), GetRoles(), GetUserProjects(id), GetUserAttributes(id), GetUserCvs(id)]);
                if (profileUser.profileData?.personalPhoto) profileUser.profileData.personalPhotoUrl = (await GetUserImage(profileUser.profileData.personalPhoto)).url;
                setRoles(userRoles);
                setUser({...profileUser, projects, attributeValues: attributes, cvs});
            } catch {
                message.error(t("messages.profileLoadError"));
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [profileUserId, auth.userId, auth.role]);

    const handleSaveProfile = async (data: UserProfile, redirect: boolean) => {
        try {
            await UpdateUser(data);
            setUser({...data, version: data.version + 1});
            message.success(t("messages.profileSaved"));
            const userId = searchParams.get("userId");
            if(redirect)
                navigate(userId ? `/profile?userId=${encodeURIComponent(userId)}&mode=view` : "/profile?mode=view");
        } catch {
            message.error(t("messages.profileSaveError"));
        }
    };
    const handleUploadImage = async (id: string, file: File) => {
        try {
            const image = await UploadUserImage(id, file);
            const imageUrl = await GetUserImage(image);
            setUser(current => ({
                ...current,
                version: current.version + 1,
                profileData: {...current.profileData, personalPhoto: image, personalPhotoUrl: imageUrl.url}
            }));
            message.success(t("messages.photoUploaded"));
        } catch {
            message.error(t("messages.photoUploadError"));
        }
    };
    const handleSearchTechnologies = async (query: string) => {
        if (!query.trim()) return setTechnologyOptions([]);
        try {
            setTechnologyOptions((await SearchTechnologies(query)).map(({name}) => ({value: name, label: name})));
        } catch {
            message.error(t("messages.techSearchError"));
        }
    };
    return {
        user,
        setUser,
        roles,
        currentUserId,
        currentUserRole,
        loading,
        isViewMode,
        technologyOptions,
        handleSaveProfile,
        handleUploadImage,
        handleSearchTechnologies,
        loadAttributeImage: GetAttributeImage
    };
}
