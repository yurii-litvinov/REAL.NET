//(* Copyright 2017 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

//namespace Repo.Metamodels

//open Repo.DataLayer
//open Repo.CoreMetamodel
//open Repo.InfrastructureMetamodel

//(*
///// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
//type FeatureTestModelBuilder() =
//   interface IModelBuilder with
//       member this.Build(repo: IDataRepository): unit =
//           let infrastructure = InfrastructureSemantic(repo)
//           let metamodel = repo.Model "FeatureMetamodel"
//           let infrastructureMetamodel = infrastructure.Metamodel.Model

//           let model = repo.CreateModel("FeatureTestModel", metamodel)

//           ()
//*)