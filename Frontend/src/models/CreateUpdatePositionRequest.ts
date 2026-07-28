import {Technology} from "./Technology";
import {ExpertiseLevel} from "./ExpertiseLevel";
import {CreateUpdateAccessRuleRequest} from "./CreateUpdateAccessRuleRequest";

export interface CreateUpdatePositionRequest {
    title: string;
    description: string;
    expertiseLevel: ExpertiseLevel;
    accessRules: CreateUpdateAccessRuleRequest[];
    technologies: Technology[];
    maxProjects: number;
    restricted: boolean;
    version: number;
}