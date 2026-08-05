import os
from odoo import models, fields, api
from odoo.exceptions import UserError
from .api_client import CVApiClient, CVApiClientError

class Position(models.Model):
    _name = "position"
    _description = "Position"
    _rec_name = "title"  

    backend_id = fields.Char(string="Backend ID", readonly=True)
    api_token = fields.Char(string="API Token")
    api_url_display = fields.Char(string="Current API URL", compute="_compute_api_url_display")

    title = fields.Char(string="Title", readonly=True)
    description = fields.Text(string="Description", readonly=True)
    expertise_level = fields.Selection([
        ('junior', 'Junior'),
        ('middle', 'Middle'),
        ('senior', 'Senior')
    ], string="Expertise Level", readonly=True)
    
    max_projects = fields.Integer(string="Max Projects", readonly=True)
    restricted = fields.Boolean(string="Restricted Access", readonly=True)
    restricted_display = fields.Char(string="Restricted Access", compute="_compute_restricted_display", readonly=True)
    created_at = fields.Datetime(string="Created At", readonly=True)
    technologies = fields.Char(string="Technologies", readonly=True)
    
    access_rule_ids = fields.One2many('position.access.rule', 'position_id', string="Access Rules", readonly=True)

    @api.depends('api_token')
    def _compute_api_url_display(self):
        for record in self:
            record.api_url_display = self._get_api_url()

    @api.depends('restricted')
    def _compute_restricted_display(self):
        for record in self:
            record.restricted_display = "Yes" if record.restricted else "No"

    def _get_api_url(self):
        env_url = os.environ.get('BACKEND_API_URL')
        if env_url:
            return env_url
        return self.env['ir.config_parameter'].sudo().get_param('position_import.api_url')

    def action_import(self):
        for record in self:
            if not record.api_token:
                raise UserError("API Token is missing. Please provide a valid token.")
            
            api_url = self._get_api_url()
            client = CVApiClient(record.api_token, api_url)
            
            try:
                raw_data = client.fetch_position_data()
                parsed_data = client.parse_position_data(raw_data)
                
                record.write({
                    'backend_id': parsed_data.get('backend_id'),
                    'title': parsed_data.get('title'),
                    'description': parsed_data.get('description'),
                    'max_projects': parsed_data.get('max_projects'),
                    'restricted': parsed_data.get('restricted'),
                    'created_at': parsed_data.get('created_at'),
                    'technologies': parsed_data.get('technologies'),
                })
                
                if 'expertise_level' in parsed_data:
                    record.expertise_level = parsed_data['expertise_level']
                
                record.access_rule_ids.unlink()
                rules_to_create = parsed_data.get('access_rules', [])
                if rules_to_create:
                    record.access_rule_ids = [(0, 0, rule) for rule in rules_to_create]
                
            except CVApiClientError as e:
                raise UserError(str(e))
            except Exception as e:
                raise UserError(f"An unexpected error occurred: {str(e)}")
                
        return True