import React, {useEffect, useState} from "react";
import {useNavigate} from "react-router";
import {useTranslation} from "react-i18next";
import {message, Spin} from "antd";
import CvListTable from "../Components/CvListTable/CvListTable";
import {CvBasicModel} from "../models/CvBasicModel";
import {GetAllCvs} from "../Services/CvService";

export default function AllCvsPage() {
    const {t} = useTranslation();
    const [cvs, setCvs] = useState<CvBasicModel[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const loadCvs = async () => {
            try {
                const response = await GetAllCvs();
                setCvs(response);
            } catch {
                message.error(t("messages.cvLoadError"));
            } finally {
                setLoading(false);
            }
        };

        loadCvs();
    }, []);

    const handleRowClick = (cv: CvBasicModel) => {
        const userId = cv.userId ?? "";
        if (!userId) {
            message.error(t("messages.cvOpenError"));
            return;
        }

        navigate(`/cv?userId=${encodeURIComponent(userId)}&positionId=${encodeURIComponent(cv.positionId)}&mode=view`);
    };

    if (loading) {
        return <div className="position-fixed top-50 start-50 translate-middle"><Spin size="large"/></div>;
    }

    return (
        <div className="p-4">
            <CvListTable data={cvs} handleOpen={handleRowClick}/>
        </div>
    );
}
