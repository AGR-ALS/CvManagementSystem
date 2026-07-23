import {FilterOperator} from "./FilterOperator";
import {AttributeValue} from "./AttributeValue";

export interface AccessRule {
    id: string;
    filterOperator: FilterOperator,
    attributeValue: AttributeValue,
}