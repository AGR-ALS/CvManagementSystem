import {message} from "antd";
import {useTranslation} from "react-i18next";
import PositionList from "../Components/PositionsList/PositionList";
import {DeletePositions, GetPositions} from "../Services/PositionsService";
import {Position} from "../models/Position";
import CommonPage from "./CommonPage";
import {useEffect, useState} from "react";
import {useNavigate} from "react-router";
import {isRegular} from "../utils/roles";
import {useAuth} from "../Contexts/AuthContext";

export default function PositionsPage() {
    const {t} = useTranslation();
    const auth = useAuth();
    const [entities, setEntities] = useState<Position[]>([]);
    const [hideToolbar, setHideToolbar] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        GetPositions().then(setEntities);
        setHideToolbar(!auth.isLoggedIn || isRegular(auth.role));
    }, [auth.isLoggedIn, auth.role]);

    const handleCreate = () => {
        navigate("/position?mode=create");
    };

    const handleDelete = async (ids: React.Key[]) => {
        if (ids.length === 0) return;
        const stringIds = ids.map(id => String(id));
        try {
            await DeletePositions(stringIds);
            setEntities(await GetPositions());
        } catch {
            message.error(t("messages.positionsDeleteError"));
        }
    };

    const handleRowClick = (position: Position) => {
        navigate(`/position?id=${position.id}&mode=view`);
    };

    return (
        <CommonPage
            data={entities}
            TableComponent={PositionList}
            onCreate={hideToolbar ? undefined : handleCreate}
            onDelete={hideToolbar ? undefined : handleDelete}
            onRowClick={handleRowClick}
        />
    );
}
