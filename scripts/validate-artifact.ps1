[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("work-packet", "implementation-plan", "run-state", "architecture-context", "verification-report")]
    [string]$ArtifactType,

    [Parameter(Mandatory = $true)]
    [string]$InputFile,

    [Parameter(Mandatory = $false)]
    [string]$Schema
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Fail {
    param([string]$Message)
    Write-Error $Message
    exit 1
}

function Require-FrontmatterField {
    param(
        [string]$Frontmatter,
        [string]$Name,
        [string[]]$AllowedValues = @()
    )

    $escapedName = [regex]::Escape($Name)
    $match = [regex]::Match($Frontmatter, "(?m)^$escapedName\s*:\s*(.+?)\s*$")
    if (-not $match.Success) {
        Fail "Missing YAML frontmatter field: $Name"
    }

    $value = $match.Groups[1].Value.Trim()
    $value = $value.Trim([char]34).Trim([char]39)

    if ($AllowedValues.Count -gt 0 -and $value -notin $AllowedValues) {
        Fail "Invalid value for frontmatter field $$Name: $value. Allowed: $$($AllowedValues -join ', ')"
    }

    return $value
}

function Validate-JsonArtifact {
    param(
        [string]$ExpectedArtifactType,
        [string]$JsonText,
        [string]$SchemaPath
    )

    if ([string]::IsNullOrWhiteSpace($SchemaPath)) {
        Fail "Schema is required for artifact type $$ExpectedArtifactType."
    }

    if (-not (Test-Path -LiteralPath $$SchemaPath -PathType Leaf)) {
        Fail "Schema file does not exist: $$SchemaPath"
    }

    try {
        $document = $JsonText | ConvertFrom-Json -Depth 100
    }
    catch {
        Fail "Input is not valid JSON: $$($$_.Exception.Message)"
    }

    if (-not $document.PSObject.Properties.Name.Contains("artifact_type")) {
        Fail "JSON artifact is missing artifact_type."
    }

    if ($document.artifact_type -ne $ExpectedArtifactType) {
        Fail "Expected artifact_type $$ExpectedArtifactType but found $$($$document.artifact_type)."
    }

    $resolvedSchema = (Resolve-Path -LiteralPath $SchemaPath).Path

    try {
        $valid = Test-Json -Json $JsonText -SchemaFile $resolvedSchema -ErrorAction Stop
    }
    catch {
        Fail "JSON Schema validation could not run: $$($$_.Exception.Message)"
    }

    if (-not $valid) {
        Fail "JSON Schema validation failed for $$InputFile."
    }
}

function Validate-MarkdownArtifact {
    param(
        [string]$ExpectedArtifactType,
        [string]$Markdown
    )

    $match = [regex]::Match($Markdown, "\A---\s*\r?\n(?<s>.*?)\r?\n---(?:\r?\n|$$)")
    if (-not $match.Success) {
        Fail "Markdown artifact must start with YAML frontmatter delimited by ---."
    }

    $frontmatter = $match.Groups["s"].Value
    $actualType = Require-FrontmatterField -Frontmatter $frontmatter -Name "artifact_type"

    if ($actualType -ne $ExpectedArtifactType) {
        Fail "Expected artifact_type $ExpectedArtifactType but found $actualType."
    }

    [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "version")
    [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "issue_key")
    [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "generated_at")
    [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "repository_commits")

    if ($ExpectedArtifactType -eq "architecture-context") {
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "status" -AllowedValues @("selected", "needs-human"))
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "work_packet")
    }

    if ($ExpectedArtifactType -eq "verification-report") {
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "status" -AllowedValues @("pass", "fail", "needs-human"))
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "work_packet")
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "architecture_context")
        [void](Require-FrontmatterField -Frontmatter $frontmatter -Name "implementation_plan")
    }
}

if (-not (Test-Path -LiteralPath $InputFile -PathType Leaf)) {
    Fail "Input file does not exist: $InputFile"
}

$resolvedInput = (Resolve-Path -LiteralPath $InputFile).Path
$content = [System.IO.File]::ReadAllText($resolvedInput)

if ([string]::IsNullOrWhiteSpace($content)) {
    Fail "Input file is empty: $InputFile"
}

switch ($ArtifactType) {
    "work-packet" {
        Validate-JsonArtifact -ExpectedArtifactType "work-packet" -JsonText $content -SchemaPath $Schema
    }
    "implementation-plan" {
        Validate-JsonArtifact -ExpectedArtifactType "implementation-plan" -JsonText $content -SchemaPath $Schema
    }
    "run-state" {
        Validate-JsonArtifact -ExpectedArtifactType "run-state" -JsonText $content -SchemaPath $Schema
    }
    "architecture-context" {
        Validate-MarkdownArtifact -ExpectedArtifactType "architecture-context" -Markdown $content
    }
    "verification-report" {
        Validate-MarkdownArtifact -ExpectedArtifactType "verification-report" -Markdown $content
    }
    default {
        Fail "Unsupported artifact type: $ArtifactType"
    }
}

Write-Host "VALID: $ArtifactType - $resolvedInput"
exit 0