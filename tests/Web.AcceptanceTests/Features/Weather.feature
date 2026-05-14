@Weather
Feature: Weather

Scenario: Weather forecast data is displayed
    Given an authenticated user visits the weather page
    Then the weather forecast heading is "Weather"
    And the weather forecast table is displayed
    And 5 weather forecasts are shown
