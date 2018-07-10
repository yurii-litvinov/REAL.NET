(* Copyright 2017 Yurii Litvinov
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

/// Factory that creates pre-configured repository.
[<AbstractClass; Sealed>]
type RepoFactory =
    /// Method that returns initialized repository.
    static member Create() = 
        let data = new DataLayer.DataRepo() :> DataLayer.IRepo
        let build (builder: Metametamodels.IModelBuilder) =
            builder.Build data

        Metametamodels.CoreMetametamodelBuilder() |> build
        Metametamodels.LanguageMetamodelBuilder() |> build
        Metametamodels.InfrastructureMetamodelBuilder() |> build
        Metametamodels.RobotsMetamodelBuilder() |> build
        Metametamodels.RobotsTestModelBuilder() |> build
        Metametamodels.AirSimMetamodelBuilder() |> build
        Metametamodels.AirSimModelBuilder() |> build

        new FacadeLayer.Repo(data) :> IRepo

    /// Method that returns a new repository populated from a save file.
    static member Load fileName =
        let data = new DataLayer.DataRepo() :> DataLayer.IRepo
        Serializer.Deserializer.load fileName data
        new FacadeLayer.Repo(data) :> IRepo
