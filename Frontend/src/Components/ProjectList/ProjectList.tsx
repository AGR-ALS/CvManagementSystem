import {Button, Card, Tag} from "antd";
import {PlusOutlined} from "@ant-design/icons";
import {useTranslation} from "react-i18next";
import {useProjectContext} from "../../Contexts/ProjectContext";
import "./ProjectList.css";

export default function ProjectList() {
    const {t} = useTranslation();

    const {
        projects,
        handleOpenCreation,
        handleOpenProject,
        readOnly,
    } = useProjectContext();

    return (
        <div>

            <div className="project-list__container">

                {
                    projects?.map(project => (

                        <Card
                            key={project.id}
                            size="small"
                            className={`w-100 ${readOnly ? "project-list__card" : "project-list__card--clickable"}`}
                            onClick={() => {
                                if (!readOnly) {
                                    handleOpenProject(project);
                                }
                            }}
                        >

                            <div>
                                <b>
                                    {project.name}
                                </b>
                            </div>

                            <div className="project-list__tags">

                                {
                                    project.technologies.map(
                                        tech => (
                                            <Tag
                                                key={tech.name}
                                            >
                                                {tech.name}
                                            </Tag>
                                        )
                                    )
                                }

                            </div>

                        </Card>

                    ))
                }

                {!readOnly && (
                    <Button
                        type="dashed"
                        icon={<PlusOutlined/>}
                        className="project-list__add-btn"
                        onClick={() =>
                            handleOpenCreation()
                        }
                    >
                        {t("project.create")}
                    </Button>
                )}

            </div>

        </div>
    );
}
