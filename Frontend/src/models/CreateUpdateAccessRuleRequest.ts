import {FilterOperator} from "./FilterOperator";
import {CreateUpdateAttributeValueRequest} from "./CreateUpdateAttributeValueRequest";
import {AttributeDataType} from "./AttributeDataType";

export interface CreateUpdateAccessRuleRequest {
    filterOperator: FilterOperator;
    attributeValue: CreateUpdateAttributeValueRequest;
    attributeDataType: AttributeDataType;
    attributeValueId: string;
}