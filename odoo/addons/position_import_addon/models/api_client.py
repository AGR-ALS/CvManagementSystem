import json
from datetime import datetime

import requests

from .datatypes import DATA_TYPE_MAP
from .filter_operators import FILTER_OPERATOR_MAP

class CVApiClientError(Exception):
    pass

class CVApiClient:
    def __init__(self, token, base_url):
        self.token = token
        self.base_url = base_url
        self.headers = {
            "X-Api-Token": self.token,
            "Accept": "application/json",
        }

    def fetch_position_data(self):
        try:
            response = requests.get(self.base_url, headers=self.headers, timeout=10)
            response.raise_for_status()
            return response.json()
        except requests.exceptions.HTTPError:
            if response.status_code == 401:
                raise CVApiClientError("Unauthorized: Invalid or expired API Token.")
            raise CVApiClientError(f"HTTP Error {response.status_code}: {response.text}")
        except requests.exceptions.ConnectionError:
            raise CVApiClientError(f"Connection Error: Could not reach API at {self.base_url}")
        except requests.exceptions.RequestException as req_err:
            raise CVApiClientError(f"Request Error: {str(req_err)}")
        except json.JSONDecodeError as json_err:
            raise CVApiClientError(f"Failed to parse API response as JSON: {str(json_err)}")
        except Exception as e:
            raise CVApiClientError(f"An unexpected error occurred: {str(e)}")

    def parse_position_data(self, data):
        parsed = {
            "backend_id": str(data.get("id", "")),
            "title": data.get("title", "No Title"),
            "description": data.get("description", ""),
            "max_projects": data.get("maxProjects", 0),
            "restricted": data.get("restricted", False),
        }

        created_at_str = data.get("createdAt")
        if created_at_str:
            clean_date_str = created_at_str.replace("Z", "+00:00")
            dt = datetime.fromisoformat(clean_date_str)
            parsed["created_at"] = dt.strftime("%Y-%m-%d %H:%M:%S")

        expertise_raw = data.get("expertiseLevel")
        expertise = ""

        if isinstance(expertise_raw, int):
            expertise_map = {
                0: "junior",
                1: "middle",
                2: "senior",
            }
            expertise = expertise_map.get(expertise_raw, "")
        elif isinstance(expertise_raw, str):
            expertise = expertise_raw.lower()

        if expertise in ["junior", "middle", "senior"]:
            parsed["expertise_level"] = expertise

        tech_list = [tech.get("name", "") for tech in data.get("technologies", [])]
        parsed["technologies"] = ", ".join(filter(None, tech_list))

        rules = []

        for item in data.get("aggregatedAttributeValues", []):
            rule = item.get("accessRule", {})
            attr_value = rule.get("attributeValue", {})
            attr_def = attr_value.get("attributeDefinition", {})

            raw_value = attr_value.get("value")
            formatted_value = self._format_access_rule_value(raw_value)

            raw_data_type = attr_def.get("dataType")
            data_type_str = DATA_TYPE_MAP.get(raw_data_type, str(raw_data_type))

            raw_filter_op = rule.get("filterOperator")
            filter_op_str = FILTER_OPERATOR_MAP.get(raw_filter_op, str(raw_filter_op))

            rules.append(
                {
                    "attribute_name": attr_def.get("name", "Unknown"),
                    "attribute_data_type": data_type_str,
                    "filter_operator": filter_op_str,
                    "value": formatted_value,
                    "aggregated_value": self._format_access_rule_value(
                        item.get("aggregatedValue")
                    ),
                }
            )

        parsed["access_rules"] = rules

        return parsed

    def _format_access_rule_value(self, raw_value):
        if isinstance(raw_value, dict):
            if "start" in raw_value and "end" in raw_value:
                return f"{raw_value['start']} - {raw_value['end']}"

            if "startValue" in raw_value and "endValue" in raw_value:
                return f"{raw_value['startValue']} - {raw_value['endValue']}"

            if "value" in raw_value and "oneOfManyValueId" in raw_value:
                return str(raw_value["value"])

            return json.dumps(raw_value, ensure_ascii=False)

        if isinstance(raw_value, bool):
            return "Yes" if raw_value else "No"

        if raw_value is not None:
            return str(raw_value)

        return "N/A"