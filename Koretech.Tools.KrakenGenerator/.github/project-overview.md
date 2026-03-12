
# Introduction

This document describes the Kraken and KamlBoGen projects, their purpose and architectural guidelines to follow when designing and implementing them.
   - These projects are being undertaken by Kore Technologies (aka 'Kore').
   - Kore's products include Kore Commerce, a web-based storefront application, and Kore Integrate, an integration hub that has historically allowed Unidata back office systems 
     to be integrated with Kore Commerce.
   - It is Kore's intention to migrate Kore Commerce to a modern .NET-based implementation.
   - These projects are part of that effort.

# Kraken project overview

## 1. Purpose and Background
  - The Kraken project will migrate Kore Commerce from its current .NET Framework implementation to .NET Core.
  - The existing business objects (BOs) in Kore Commerce use SubSonic for persistence.  This will be replaced with Entity Framework (EF) Core.
  - The existing BOs are partially code generated, driven by metadata defined in KAMLBO files.  
    - KAMLBO is an XML-based format.
    - KAMLBO describes BOs, their properties and relationships, among other things.
    - Kraken will use an updated KAMLBO format (Kraken KAMLBO) which is more compact, but still XML-based and will be able to express the same metadata as the existing format.
    - Kraken KAMLBO can be created using a tool called Kamlbo2Krakenbo, which is part of the Kraken project.

## 2. Architecture and Approach
  - The Kraken project will be implemented as a new codebase, separate from the existing Kore Commerce codebase.
  - The new codebase will be implemented in C# and the latest version of .NET and Entity Framework.
  - The existing code will be migrated incrementally, as described in [Slice-Based Migration Approach](..\docs\Slice-Based Migration Approach.md)
  - During the migration, the existing code and new code will interoperate through an API layer, which will be implemented as part of the Kraken project.
  
# KamlBoGen project overview

## 1. Purpose and Scope
   - The Kraken project will include a new code generator that will generate BOs and EF-based data access layers (DALs) based on a newer version of the existing KAMLBO metadata.
   - The purpose of the KamlBoGen project is to implement this code generator.
   - The code generator is implemented in the Koretech.Tools.KrakenGenerator project in the Kraken repository.
   - The scope of this project includes:
     - Ingesting KAMLBO files in the new format (Kraken KAMLBO).
     - Generating BOs and EF-based DALs that are functionally equivalent to those currently in use in Kore Commerce, with the same properties and behavior.
     - Implementing an improved architecture for business logic and persistence operations.
     - Providing extension points for business logic that we need to preserve, e.g. for processing incoming data beyond simple persistence operations.

## 2. Architecture and Design
   - The primary product of this project is the code generator described above.
   - This code generator is a CLI application implemented in the latest version of .NET.
   - It consists of two layers, a model layer that holds in-memory representations of the metadata, and an emitter layer that generates code from the model.
   - It should be designed so that the model layer is reusable for other purposes.
   - The model layer should include a validation mechanism that verifies all needed data has been loaded into the model prior to generation, and that the data is valid.
   - The code emitters should be designed to reuse common code templates where possible.
   - The emitters should create a local model object containing metadata needed to generate a specific file, e.g. an entity or DB context.
   - The emitters should use this local model rather than going back to the KAMLBO model for any information needed during code generation.
   - KamlBoGen should include diagnostic logging that displays progress as the model is loaded, validation results, and code generation progress.
   - Any errors encountered during loading, validation or generation should be logged to allow for troubleshooting and resolution.

## 3. Project State as of 2026-03-06
   - The project is being revived after an earlier attempt at Kraken.
   - Significant improvements are needed to support all of the KAMLBO metadata and to generate code that is functionally equivalent to the existing BOs and DALs.
   - A similar code generation project was implemented for the Importer project (located at C:\Dev\Source\Repos\KommerceServer-Import\Koretech.Importer.Tools).
     - The KAMLBO model in that project is more complete than the one currently in KamlBoGen, and will be extracted and adapted for use in both projects.

   If any aspect of this is unclear or incomplete, do not make assumptions, ask what you should do.
   
