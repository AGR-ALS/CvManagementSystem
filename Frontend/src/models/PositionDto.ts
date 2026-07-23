import {FilterOperator} from "./FilterOperator";
import {AttributeDataType} from "./AttributeDataType";
import {Technology} from "./Technology";
import {ExpertiseLevel} from "./ExpertiseLevel";

import {CreateUpdateAttributeValueRequest} from "./CreateUpdateAttributeValueRequest";

export interface CreateUpdateAccessRuleRequest {
    filterOperator: FilterOperator;
    attributeValue: CreateUpdateAttributeValueRequest;
    attributeDataType: AttributeDataType;
    attributeValueId: string;
}

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