namespace Sharpino.Sample

open Sharpino.Sample.Models.TagsModel

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
        static member StorageName =
            "_tags"
        static member Version =
            "_01"
        static member SnapshotsInterval =
            15
        member this.AddTag(t: Tag) =
            ResultCE.result {
                let! tags = this.Tags.AddTag(t)
                return
                    {
                        this with
                            Tags = tags
                    }
            }

        member this.RemoveTag(id: Guid) =
            ResultCE.result {
                let! tags = this.Tags.RemoveTag(id)
                return
                    {
                        this with
                            Tags = tags
                    }
            }
        member this.GetTags() = this.Tags.GetTags()
