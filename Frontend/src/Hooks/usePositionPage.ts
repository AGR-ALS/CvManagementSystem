import {useEffect, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {useNavigate, useSearchParams} from "react-router";
import {Position} from "../models/Position";
import {CreateUpdatePositionRequest} from "../models/PositionDto";
import {AttributeDefinition} from "../models/AttributeDefinition";
import {CreatePosition, GetPosition, UpdatePosition} from "../Services/PositionsService";
import {SearchTechnologies} from "../Services/TechnologiesService";
import {GetAttributeDefinitions} from "../Services/AttributeService";
import {ResolveCv} from "../Services/CvService";
import {isRegular} from "../utils/roles";
import {useAuth} from "../Contexts/AuthContext";

export type PositionPageMode = "create" | "edit" | "clone" | "view";

export function usePositionPage() {
    const {t} = useTranslation();
    const auth = useAuth();
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const idParam = searchParams.get("id");
    const modeParam = searchParams.get("mode") as PositionPageMode | null;
    const [position, setPosition] = useState<Position | null>(null);
    const [mode, setMode] = useState<PositionPageMode>("create");
    const [loading, setLoading] = useState(true);
    const [canEdit, setCanEdit] = useState(false);
    const [technologyOptions, setTechnologyOptions] = useState<{ value: string; label: string }[]>([]);
    const [attributeDefinitions, setAttributeDefinitions] = useState<AttributeDefinition[]>([]);

    useEffect(() => {
        if (!idParam) {
            setPosition(null);
            setAttributeDefinitions([]);
            setCanEdit(false);
            setLoading(false);
            return;
        }
        const load = async () => {
            setLoading(true);
            try {
                setCanEdit(Boolean(auth.role && !isRegular(auth.role)));
                setAttributeDefinitions(await GetAttributeDefinitions());
                setPosition(await GetPosition(idParam));
            } catch{
                message.error(t("messages.positionLoadError"));
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [idParam, auth.role]);

    useEffect(() => {
        if (modeParam === "clone" && position) {
            setMode("clone");
            setPosition(x => x ? {...x, id: ""} : x);
        } else if (idParam) {
            setMode(modeParam === "edit" ? "edit" : "view");
        } else {
            setMode("create");
        }
    }, [modeParam, idParam, position]);

    const onSearchTechnologies = async (query: string) => {
        if (!query.trim()) return setTechnologyOptions([]);
        try {
            const technologies = await SearchTechnologies(query);
            setTechnologyOptions(technologies.map(({name}) => ({value: name, label: name})));
        } catch {
            message.error(t("messages.techSearchError"));
        }
    };

    const onSave = async (request: CreateUpdatePositionRequest, positionId?: string) => {
        try {
            if (mode === "create" || mode === "clone") await CreatePosition(request);
            else if (mode === "edit" && positionId) await UpdatePosition(positionId, request);
            message.success(t("messages.positionSaved"));
            navigate(mode === "edit" && positionId ? `/position?id=${positionId}&mode=view` : "/positions");
        } catch {
            message.error(t("messages.positionSaveError"));
        }
    };

    const onGenerateCv = async () => {
        if (!position) return;
        try {
            const userId = auth.userId;
            if (!userId) return;
            await ResolveCv(userId, position.id);
            navigate(`/cv?userId=${encodeURIComponent(userId)}&positionId=${encodeURIComponent(position.id)}&mode=view`);
        } catch {
            message.error(t("messages.cvGenerateError"));
        }
    };

    return {
        idParam,
        position,
        mode,
        loading,
        canEdit,
        technologyOptions,
        attributeDefinitions,
        onSearchTechnologies,
        onSave,
        onGenerateCv,
        onClone: () => idParam && navigate(`/position?id=${idParam}&mode=clone`),
        onCancel: () => navigate("/positions"),
        onEdit: () => idParam && navigate(`/position?id=${idParam}&mode=edit`)
    };
}
