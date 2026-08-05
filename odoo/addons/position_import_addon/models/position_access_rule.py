from odoo import fields, models

class PositionAccessRule(models.Model):
    _name = "position.access.rule"
    _description = "Position Access Rule"
    _order = "id desc"

    position_id = fields.Many2one(
        "position",
        string="Position",
        required=True,
        ondelete="cascade",
    )

    attribute_name = fields.Char(string="Attribute Name", readonly=True)
    attribute_data_type = fields.Char(string="Data Type", readonly=True)
    filter_operator = fields.Char(string="Filter Operator", readonly=True)
    value = fields.Char(string="Value", readonly=True)

    aggregated_value = fields.Char(
        string="Aggregated Value",
        readonly=True,
        help="Aggregated value is taken from submitted CVs and calculated as the average for numeric values, the most common value for strings, text, booleans, dates, and options, image presence for images, and the longest time period for periods.",
    )