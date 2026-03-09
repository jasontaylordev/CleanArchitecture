@FetchData
Feature: Fetch Data

Scenario: Weather forecast data is displayed
    Given an authenticated user visits the fetch data page
    Then the weather forecast heading is "Weather forecast"
    And the weather forecast table is displayed
    And 5 weather forecasts are shown
