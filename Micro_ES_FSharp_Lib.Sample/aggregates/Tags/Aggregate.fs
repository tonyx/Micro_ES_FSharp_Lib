namespace Tonyx.EventSourcing.Sample

open Tonyx.EventSourcing.Sample.Tags.Models.TagsModel
open Tonyx.EventSourcing.Sample
open System
open FsToolkit.ErrorHandling

module TagsAggregate =
    type TagsAggregate =
        {
            Tags: Tags
        }
        static member Zero =
            {
                Tags = Tags.Zero
            }
        // storagename _MUST_ be unique for each aggregate and the relative lock object 
        // must be added in syncobjects map in Conf.fs
        static member StorageName =
            "_tags"
        static member Version =
            "_01"
        member this.AddTag(t: Tag) =
            ResultCE.result {
                let! result = this.Tags.AddTag(t)
                let result =
                    {
                        this with
                            Tags = result
                    }
                return result
            }
        member this.RemoveTag(id: Guid) =
            ResultCE.result {
                let! result = this.Tags.RemoveTag(id)
                let result =
                    {
                        this with
                            Tags = result
                    }
                return result
            }
        member this.GetTags() = this.Tags.GetTags()