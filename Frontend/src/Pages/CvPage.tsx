import {Spin} from "antd";
import {useTranslation} from "react-i18next";
import {CvFormContext} from "../Contexts/CvFormContext";
import ViewCvForm from "../Components/CvForm/ViewCvForm";
import EditCvForm from "../Components/CvForm/EditCvForm";
import {useCvPage} from "../Hooks/useCvPage";

export default function CvPage() {
    const {t} = useTranslation();
    const page = useCvPage();
    if (page.loading) return <div className="position-fixed top-50 start-50 translate-middle"><Spin size="large"/></div>;
    if (!page.cv) return <div>{t("app.notFound")}</div>;

    return (
        <CvFormContext.Provider value={{...page, cv: page.cv}}>
            {page.isViewMode ? <ViewCvForm/> : <EditCvForm/>}
        </CvFormContext.Provider>
    );
}
