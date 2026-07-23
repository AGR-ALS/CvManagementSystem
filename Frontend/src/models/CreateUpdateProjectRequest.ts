import {Technology} from "./Technology";

export interface CreateUpdateProjectRequest {
    name: string;
    description: string;
    technologies: Technology[];
    version: number;
}
