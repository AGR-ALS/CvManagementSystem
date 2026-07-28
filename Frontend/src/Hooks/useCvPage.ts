import {useEffect, useMemo, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {useNavigate, useSearchParams} from "react-router";
import {Cv} from "../models/Cv";
import {Project} from "../models/Project";
import {CvAttributeRow} from "../Components/CvAttributeTable/CvAttributeTable";
import {AttributeDataType} from "../models/AttributeDataType";
import {CreateUpdateAttributeValueRequest} from "../models/CreateUpdateAttributeValueRequest";
import {GetUserBasicInfo, GetUserImage} from "../Services/UserService";
import {CheckIfUserLikedCv, DeleteCv, GetCv, LikeCv, PublishCv, RemoveLike, UpdateCv} from "../Services/CvService";
import {
    AddAttributeValueToUser,
    GetAttributeImage,
    GetUserAttributes,
    UpdateAttributeValueToUser
} from "../Services/AttributeService";
import {GetCvProjects, GetUserProjects} from "../Services/ProjectService";
import {GetPosition} from "../Services/PositionsService";
import {isAdmin, isRegular} from "../utils/roles";
import {useAuth} from "../Contexts/AuthContext";

export function useCvPage() {
    const {t} = useTranslation();
    const auth = useAuth();
    const [params] = useSearchParams();
    const navigate = useNavigate();
    const userId = params.get("userId"), positionId = params.get("positionId");
    const isViewMode = params.get("mode") !== "edit";
    const [cv, setCv] = useState<Cv | null>(null);
    const [loading, setLoading] = useState(true);
    const [photo, setPhoto] = useState<string>();
    const [canEdit, setCanEdit] = useState(false);
    const [regular, setRegular] = useState(true);
    const [currentId, setCurrentId] = useState("");
    const [liked, setLiked] = useState(false);
    const [projects, setProjects] = useState<Project[]>([]);
    const [selectorOpen, setSelectorOpen] = useState(false);
    const [editingRow, setEditingRow] = useState<CvAttributeRow | null>(null);
    const [attributeOpen, setAttributeOpen] = useState(false);

    useEffect(() => {
        const load = async () => {
            if (!userId || !positionId) {
                navigate("/profile?mode=view");
                return;
            }
            try {
                const cid = auth.userId, me = auth.role,
                    data = await GetCv(userId, positionId);
                const [user, position, all, selected, attrs] = await Promise.all([GetUserBasicInfo(userId), GetPosition(data.positionId), GetUserProjects(userId), GetCvProjects(data.id), GetUserAttributes(userId)]);
                if (user.profileData?.personalPhoto) setPhoto((await GetUserImage(user.profileData.personalPhoto)).url);
                const result = {
                    ...data,
                    user: {...user, projects: all, attributeValues: attrs},
                    position,
                    projects: selected
                } as Cv;
                setCv(result);
                setProjects(selected);
                setCurrentId(cid);
                setRegular(isRegular(me));
                setCanEdit(userId === cid || isAdmin(me));
                if (cid && !isRegular(me)) setLiked(await CheckIfUserLikedCv(data.id, cid));
            } catch {
                message.error(t("messages.cvLoadError"));
                navigate("/profile?mode=view");
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [userId, positionId, navigate, auth.userId, auth.role]);

    const save = async () => {
        if (!cv) return;
        try {
            await UpdateCv(cv.user.id, cv.position.id, {
                id: cv.id,
                projectsIds: projects.map(p => p.id),
                version: cv.version
            });
            message.success(t("messages.cvSaved"));
            navigate(`/cv?userId=${encodeURIComponent(cv.user.id)}&positionId=${encodeURIComponent(cv.position.id)}&mode=view`)
        } catch {
            message.error(t("messages.cvSaveError"))
        }
    };
    const remove = async () => {
        if (!cv) return;
        try {
            await DeleteCv(cv.id);
            message.success(t("messages.cvDeleted"));
            navigate("/profile?mode=view")
        } catch {
            message.error(t("messages.cvDeleteError"))
        }
    };
    const saveAttribute = async (type: AttributeDataType, data: CreateUpdateAttributeValueRequest) => {
        if (!cv || !editingRow) return;
        try {
            if (editingRow.userAttribute) await UpdateAttributeValueToUser(editingRow.userAttribute.id, type, data); else await AddAttributeValueToUser(cv.user.id, type, data);
            const attrs = await GetUserAttributes(cv.user.id);
            setCv(x => x ? {...x, user: {...x.user, attributeValues: attrs}} : x);
            setAttributeOpen(false);
            setEditingRow(null);
            message.success(t("messages.attributeSavedOnCv"))
        } catch {
            message.error(t("messages.attributeSaveOnCvError"))
        }
    };
    const toggleLike = async () => {
        if (!cv || !currentId) return;
        try {
            if (liked) await RemoveLike(cv.id, currentId); else await LikeCv(cv.id, currentId);
            setCv(x => x ? {...x, likes: x.likes + (liked ? -1 : 1)} : x);
            setLiked(!liked)
        } catch {
            message.error(t("messages.likeToggleError"))
        }
    };
    const publish = async () => {
        if (!cv) return;
        try {
            await PublishCv(cv.id);
            setCv(x => x ? {...x, published: true} : x);
            message.success(t("messages.cvPublished"))
        } catch {
            message.error(t("messages.cvPublishError"))
        }
    };
    return {
        cv,
        loading,
        photo,
        canEdit,
        regular,
        liked,
        isViewMode,
        projects,
        selectorOpen,
        editingRow,
        attributeOpen,
        loadAttributeImage: (key: string) => GetAttributeImage(key).then(r => r.url),
        save,
        remove,
        saveAttribute,
        toggleLike,
        publish,
        onEdit: () => userId && positionId && navigate(`/cv?userId=${encodeURIComponent(userId)}&positionId=${encodeURIComponent(positionId)}&mode=edit`),
        onCancel: () => navigate(-1),
        openSelector: () => setSelectorOpen(true),
        closeSelector: () => setSelectorOpen(false),
        addProjects: (ids: string[]) => cv && setProjects(x => [...x, ...cv.user.projects.filter(p => ids.includes(p.id))].slice(0, cv.position.maxProjects)),
        removeProject: (id: string) => setProjects(x => x.filter(p => p.id !== id)),
        openAttribute: (row: CvAttributeRow) => {
            setEditingRow(row);
            setAttributeOpen(true)
        },
        closeAttribute: () => {
            setAttributeOpen(false);
            setEditingRow(null)
        }
    };
}
