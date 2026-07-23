import {Button, Tag} from "antd";
import {useTranslation} from "react-i18next";
import {Project} from "../../models/Project";
import "./CvProjectList.css";

interface Props {
    selectedProjects: Project[];
    maxProjects: number;
    onRemove?: (id: string) => void;
    onAdd?: () => void;
}

export default function CvProjectList({selectedProjects, maxProjects, onRemove, onAdd}: Props) {
    const {t} = useTranslation();
    return (
        <div className="cv-project-list__container">
            {selectedProjects.map((project) => (
                <div
                    key={project.id}
                    className="cv-project-list__card"
                >
                    <div className="cv-project-list__header">
                        <strong>{project.name}</strong>
                        {onRemove && (
                            <Button type="text" danger onClick={() => onRemove(project.id)}>
                                ✕
                            </Button>
                        )}
                    </div>
                    <div className="cv-project-list__text">
                        {project.description}
                    </div>
                    <div className="cv-project-list__tags">
                        {project.technologies.map((technology) => (
                            <Tag key={technology.name}>{technology.name}</Tag>
                        ))}
                    </div>
                </div>
            ))}

            {onAdd && selectedProjects.length < maxProjects && (
                <Button type="dashed" onClick={onAdd} className="cv-project-list__add-btn">
                    {t("cvProjectSelect.title")}
                </Button>
            )}
        </div>
    );
}
