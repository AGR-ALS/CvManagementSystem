import {AccessRule} from "./AccessRule";
import {Technology} from "./Technology";
import {ExpertiseLevel} from "./ExpertiseLevel";

export interface Position {
    id: string;
    title: string;
    description: string;
    expertiseLevel: ExpertiseLevel;
    accessRules: AccessRule[];
    technologies: Technology[];
    maxProjects: number;
    restricted: boolean;
    createdAt: string;
    version: number;
}