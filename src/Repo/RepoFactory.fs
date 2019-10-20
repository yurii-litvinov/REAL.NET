(* Copyright 2017-2019 Yurii Litvinov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. *)

namespace Repo

open Repo.InfrastructureMetamodel
open Repo.Facade

/// Factory that creates pre-configured repository.
[<AbstractClass; Sealed>]
type RepoFactory =
    /// Method that returns initialized repository.
    static member Create() = 
        let repo = InfrastructureMetamodelRepoFactory.Create ()
        let factory = FacadeFactory(repo)
        let pool = FacadePool(factory)
        //let (~+) (creator: DataLayer.IModelCreator) =
        //    creator.CreateIn data

        //+Metamodels.RobotsMetamodelCreator()
        //+Metamodels.RobotsTestModelCreator()
        ////Metamodels.AirSimMetamodelBuilder() |> add
        ////Metamodels.AirSimModelBuilder() |> add
        ////Metamodels.FeatureMetamodelBuilder() |> add
        ////Metamodels.FeatureTestModelBuilder() |> add

        new Facade.Repo(pool, repo) :> IRepo

    ///// Method that returns a new repository populated from a save file.
    //static member Load fileName =
    //    let data = new DataLayer.DataRepo() :> DataLayer.IDataRepository
    //    Serializer.Deserializer.load fileName data
    //    new FacadeLayer.Repo(data) :> IRepo

    ///// Method that returns repository with infrastructure metamodel only.
    //static member CreateEmpty () =
    //    let data = new DataLayer.DataRepo() :> DataLayer.IDataRepository
    //    let (~+) (creator: DataLayer.IModelCreator) =
    //        creator.CreateIn data

    //    +CoreMetamodel.CoreMetamodelCreator()
    //    +AttributeMetamodel.AttributeMetamodelCreator()
    //    +LanguageMetamodel.LanguageMetamodelCreator()
    //    +InfrastructureMetamodel.InfrastructureMetametamodelCreator()
    //    +InfrastructureMetamodel.InfrastructureMetamodelCreator()

    //    new FacadeLayer.Repo(data) :> IRepo
