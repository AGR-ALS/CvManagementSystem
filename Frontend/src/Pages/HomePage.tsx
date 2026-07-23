import React, {useEffect, useState} from "react";
import {useNavigate} from "react-router";
import {useTranslation} from "react-i18next";
import {Spin, message} from "antd";
import {Position} from "../models/Position";
import {GetPopularPositions, GetPositionsAmount, GetRecentPositions} from "../Services/PositionsService";
import {GetCvsAmount} from "../Services/CvService";
import {GetCandidatesAmount, GetRecruitersAmount} from "../Services/UserService";
import PositionList from "../Components/PositionsList/PositionList";
import {CONFIG} from "../config";

export default function HomePage() {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const [popular, setPopular] = useState<Position[]>([]);
    const [recent, setRecent] = useState<Position[]>([]);
    const [positionsCount, setPositionsCount] = useState(0);
    const [cvsCount, setCvsCount] = useState(0);
    const [candidatesCount, setCandidatesCount] = useState(0);
    const [recruitersCount, setRecruitersCount] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const load = async () => {
            try {
                const [pop, rec, posAmt, cvAmt, candAmt, recAmt] = await Promise.all([
                    GetPopularPositions(CONFIG.HOME_PAGE_POSITIONS_COUNT),
                    GetRecentPositions(CONFIG.HOME_PAGE_POSITIONS_COUNT),
                    GetPositionsAmount(),
                    GetCvsAmount(),
                    GetCandidatesAmount(),
                    GetRecruitersAmount(),
                ]);
                setPopular(pop);
                setRecent(rec);
                setPositionsCount(posAmt);
                setCvsCount(cvAmt);
                setCandidatesCount(candAmt);
                setRecruitersCount(recAmt);
            } catch {
                message.error(t("home.error"));
            } finally {
                setLoading(false);
            }
        };
        load();
    }, []);

    const handleOpenPosition = (position: Position) => {
        navigate(`/position?id=${encodeURIComponent(position.id)}`);
    };

    if (loading) {
        return <div className="position-fixed top-50 start-50 translate-middle"><Spin
            size="large"/></div>;
    }

    return (
        <div className="container py-4">
            <div className="row g-3 mb-4">
                <div className="col-md-3">
                    <div className="card text-center p-3">
                        <div className="fs-1 fw-bold">{positionsCount}</div>
                        <div className="text-muted">{t("home.positions")}</div>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card text-center p-3">
                        <div className="fs-1 fw-bold">{cvsCount}</div>
                        <div className="text-muted">{t("home.cvs")}</div>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card text-center p-3">
                        <div className="fs-1 fw-bold">{candidatesCount}</div>
                        <div className="text-muted">{t("home.candidates")}</div>
                    </div>
                </div>
                <div className="col-md-3">
                    <div className="card text-center p-3">
                        <div className="fs-1 fw-bold">{recruitersCount}</div>
                        <div className="text-muted">{t("home.recruiters")}</div>
                    </div>
                </div>
            </div>

            <h5 className="mb-3 text-center">{t("home.popularPositions")}</h5>
            <PositionList data={popular} handleOpen={handleOpenPosition}/>

            <h5 className="mb-3 mt-4 text-center">{t("home.recentPositions")}</h5>
            <PositionList data={recent} handleOpen={handleOpenPosition}/>
        </div>
    );
}
