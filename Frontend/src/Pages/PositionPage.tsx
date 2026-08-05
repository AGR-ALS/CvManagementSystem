import {useState} from "react";
import {useTranslation} from "react-i18next";
import {Segmented, Spin} from "antd";
import ViewPositionForm from "../Components/PositionForm/ViewPositionForm";
import EditPositionForm from "../Components/PositionForm/EditPositionForm";
import DiscussionView from "../Components/Discussion/DiscussionView";
import {PositionFormContext} from "../Contexts/PositionFormContext";
import {usePositionPage} from "../Hooks/usePositionPage";
import {useDiscussionPolling} from "../Hooks/useDiscussionPolling";
import "./PositionPage.css";

type ActiveTab = "data" | "discussion";

export default function PositionPage() {
    const {t} = useTranslation();
    const [activeTab, setActiveTab] = useState<ActiveTab>("data");
    const page = usePositionPage();
    const discussion = useDiscussionPolling(page.idParam, activeTab);

    if (page.loading) return <div className="position-fixed top-50 start-50 translate-middle"><Spin size="large"/></div>;

    return (
        <div className="d-flex flex-column position-page__outer">
            <div className="d-flex justify-content-end mb-3 px-4">
                <Segmented<string>
                    options={[{label: t("position.positionData"), value: "data"}, {label: t("position.discussion"), value: "discussion"}]}
                    value={activeTab}
                    onChange={(value) => setActiveTab(value as ActiveTab)}
                />
            </div>
            <PositionFormContext value={{
                technologyOptions: page.technologyOptions,
                onSearchTechnologies: page.onSearchTechnologies,
                attributeDefinitions: page.attributeDefinitions,
                onGenerateCv: page.onGenerateCv,
                onGenerateApiToken: page.onGenerateApiToken,
            }}>
                {activeTab === "data" ? (
                    page.mode === "view" ?
                        <ViewPositionForm position={page.position} canEdit={page.canEdit} onEdit={page.onEdit}/> :
                        <EditPositionForm position={page.position}
                                          isCreating={page.mode === "create" || page.mode === "clone"}
                                          onSave={page.onSave} onClone={page.mode === "edit" ? page.onClone : undefined}
                                          onCancel={page.onCancel}/>
                ) : <div className="px-4"><DiscussionView positionId={page.idParam} {...discussion} /></div>}
            </PositionFormContext>
        </div>
    );
}
