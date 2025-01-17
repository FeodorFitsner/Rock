﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.UniversalSearch;
using Rock.UniversalSearch.IndexModels;
using Rock.Web.Cache;
using Rock.Lava;

namespace Rock.Model
{
    /// <summary>
    /// Represents an event item for one or more event calendars.
    /// </summary>
    [RockDomain( "Event" )]
    [Table( "EventItem" )]
    [DataContract]
    public partial class EventItem : Model<EventItem>, IHasActiveFlag, IRockIndexable
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the Name of the EventItem. This property is required.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the Name of the EventItem.
        /// </value>
        [Required]
        [MaxLength( 100 )]
        [DataMember( IsRequired = true )]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Summary of the EventItem.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the summary of the EventItem.
        /// </value>
        [DataMember]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the Description of the EventItem.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the description of the EventItem.
        /// </value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.BinaryFile"/> that contains the photo of the EventItem.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> representing the Id of the <see cref="Rock.Model.BinaryFile"/> containing the photo of the EventItem.
        /// </value>
        [DataMember]
        public int? PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the URL for an external event.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the URL for an external event.
        /// </value>
        [DataMember]
        [MaxLength(200)]
        public string DetailsUrl { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        [DataMember]
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        private bool _isActive = true;

        /// <summary>
        /// Gets or sets a flag indicating if the event has been approved.
        /// </summary>
        /// <value>
        /// A <see cref="System.Boolean"/> value that is <c>true</c> if this event has been approved; otherwise <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the PersonId of the <see cref="Rock.Model.Person"/> who approved this event.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> representing the PersonId of the <see cref="Rock.Model.Person"/> who approved this event.
        /// </value>
        [DataMember]
        public int? ApprovedByPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the date this event was approved.
        /// </summary>
        /// <value>
        /// A <see cref="System.DateTime"/> representing the date that this event was approved.
        /// </value>
        [DataMember]
        public DateTime? ApprovedOnDateTime { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.BinaryFile"/> that contains the EventItem's photo.
        /// </summary>
        /// <value>
        /// The <see cref="Rock.Model.BinaryFile"/> that contains the EventItem's photo.
        /// </value>
        [DataMember]
        public virtual BinaryFile Photo { get; set; }

        /// <summary>
        /// Gets or sets a collection of the <see cref="Rock.Model.EventCalendarItem">EventCalendarItems</see> that belong to this EventItem.
        /// </summary>
        /// <value>
        /// A collection containing a collection of the <see cref="Rock.Model.EventCalendarItem">EventCalendarItems</see> that belong to this EventItem.
        /// </value>
        [LavaVisible]
        public virtual ICollection<EventCalendarItem> EventCalendarItems
        {
            get { return _eventCalenderItems ?? ( _eventCalenderItems = new Collection<EventCalendarItem>() ); }
            set { _eventCalenderItems = value; }
        }

        private ICollection<EventCalendarItem> _eventCalenderItems;

        /// <summary>
        /// Gets or sets a collection of the <see cref="Rock.Model.EventItemOccurrence">EventItemOccurrence</see> that belong to this EventItem.
        /// </summary>
        /// <value>
        /// A collection containing a collection of the <see cref="Rock.Model.EventItemOccurrence">EventItemOccurrence</see> that belong to this EventItem.
        /// </value>
        [DataMember]
        public virtual ICollection<EventItemOccurrence> EventItemOccurrences
        {
            get { return _eventItemOccurrences ?? ( _eventItemOccurrences = new Collection<EventItemOccurrence>() ); }
            set { _eventItemOccurrences = value; }
        }

        private ICollection<EventItemOccurrence> _eventItemOccurrences;

        /// <summary>
        /// Gets or sets a collection of the <see cref="Rock.Model.EventItemAudience">EventItemAudiences</see> that belong to this EventItem.
        /// </summary>
        /// <value>
        /// A collection containing a collection of the <see cref="Rock.Model.EventItemAudience">EventItemAudiences</see> that belong to this EventItem.
        /// </value>
        [LavaVisible]
        public virtual ICollection<EventItemAudience> EventItemAudiences
        {
            get { return _calendarItemAudiences ?? ( _calendarItemAudiences = new Collection<EventItemAudience>() ); }
            set { _calendarItemAudiences = value; }
        }

        private ICollection<EventItemAudience> _calendarItemAudiences;

        /// <summary>
        /// Gets or sets the approved by <see cref="Rock.Model.PersonAlias"/>.
        /// </summary>
        /// <value>
        /// The approved by person alias.
        /// </value>
        [DataMember]
        public virtual PersonAlias ApprovedByPersonAlias { get; set; }

        /// <summary>
        /// Gets the next start date time.
        /// </summary>
        /// <value>
        /// The next start date time.
        /// </value>
        [NotMapped]
        public virtual DateTime? NextStartDateTime
        {
            get
            {
                return EventItemOccurrences
                    .Select( s => s.NextStartDateTime )
                    .DefaultIfEmpty()
                    .Min();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [allows interactive bulk indexing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows interactive bulk indexing]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsInteractiveBulkIndexing => true;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the start times.
        /// </summary>
        /// <param name="beginDateTime">The begin date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <returns></returns>
        public virtual List<DateTime> GetStartTimes ( DateTime beginDateTime, DateTime endDateTime )
        {
            var result = new List<DateTime>();

            foreach ( var eventItemOccurrence in EventItemOccurrences )
            {
                result.AddRange( eventItemOccurrence.GetStartTimes( beginDateTime, endDateTime ) );
            }

            return result.Distinct().OrderBy( d => d ).ToList();
        }

        /// <summary>
        /// Get a list of all inherited Attributes that should be applied to this entity.
        /// </summary>
        /// <returns>A list of all inherited AttributeCache objects.</returns>
        public override List<AttributeCache> GetInheritedAttributes( Rock.Data.RockContext rockContext )
        {
            var calendarIds = this.EventCalendarItems.Select( c => c.EventCalendarId ).ToList();
            if ( !calendarIds.Any() )
            {
                return null;
            }

            var inheritedAttributes = new Dictionary<int, List<AttributeCache>>();
            calendarIds.ForEach( c => inheritedAttributes.Add( c, new List<AttributeCache>() ) );

            //
            // Check for any calendar item attributes that the event item inherits.
            //
            var calendarItemEntityType = EntityTypeCache.Get( typeof( EventCalendarItem ) );
            if ( calendarItemEntityType != null )
            {
                foreach ( var calendarItemEntityAttributes in AttributeCache
                    .GetByEntity( calendarItemEntityType.Id )
                    .Where( a =>
                        a.EntityTypeQualifierColumn == "EventCalendarId" &&
                        calendarIds.Contains( a.EntityTypeQualifierValue.AsInteger() ) ) )
                {
                    foreach ( var attributeId in calendarItemEntityAttributes.AttributeIds )
                    {
                        inheritedAttributes[calendarItemEntityAttributes.EntityTypeQualifierValue.AsInteger()].Add(
                            AttributeCache.Get( attributeId ) );
                    }
                }
            }

            //
            // Walk the generated list of attribute groups and put them, ordered, into a list
            // of inherited attributes.
            //
            var attributes = new List<AttributeCache>();
            foreach ( var attributeGroup in inheritedAttributes )
            {
                foreach ( var attribute in attributeGroup.Value.OrderBy( a => a.Order ) )
                {
                    attributes.Add( attribute );
                }
            }

            return attributes;
        }

        /// <summary>
        /// Get any alternate Ids that should be used when loading attribute value for this entity.
        /// </summary>
        /// <param name="rockContext"></param>
        /// <returns>
        /// A list of any alternate entity Ids that should be used when loading attribute values.
        /// </returns>
        public override List<int> GetAlternateEntityIds( RockContext rockContext )
        {
            //
            // Find all the calendar Ids this event item is present on.
            //
            return this.EventCalendarItems.Select( c => c.Id ).ToList();
        }
        #endregion

        #region Indexing Methods
        /// <summary>
        /// Bulks the index documents.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void BulkIndexDocuments()
        {
            var indexableItems = new List<IndexModelBase>();

            var eventItems = new EventItemService( new RockContext() )
                                .GetIndexableActiveItems()
                                .Include( i => i.EventItemAudiences )
                                .Include( i => i.EventItemOccurrences )
                                .Include( i => i.EventItemOccurrences.Select( s => s.Schedule ) )
                                .Include( i => i.EventCalendarItems.Select( c => c.EventCalendar ) )
                                .AsNoTracking()
                                .ToList();

            int recordCounter = 0;
            foreach ( var eventItem in eventItems )
            {
                var indexableEventItem = EventItemIndex.LoadByModel( eventItem );

                if ( indexableEventItem.IsNotNull() )
                {
                    indexableItems.Add( indexableEventItem );
                }

                recordCounter++;

                if ( recordCounter > 100 )
                {
                    IndexContainer.IndexDocuments( indexableItems );
                    indexableItems = new List<IndexModelBase>();
                    recordCounter = 0;
                }
            }

            IndexContainer.IndexDocuments( indexableItems );
        }

        /// <summary>
        /// Indexes the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void IndexDocument( int id )
        {
            var eventItemEntity = new EventItemService( new RockContext() ).Get( id );

            // Check to ensure that the event item is on a calendar that is indexed
            if ( eventItemEntity.EventCalendarItems.Any( c => c.EventCalendar.IsIndexEnabled ) )
            {
                var indexItem = EventItemIndex.LoadByModel( eventItemEntity );
                IndexContainer.IndexDocument( indexItem );
            }
        }

        /// <summary>
        /// Deletes the indexed document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteIndexedDocument( int id )
        {
            Type indexType = Type.GetType( "Rock.UniversalSearch.IndexModels.EventItemIndex" );
            IndexContainer.DeleteDocumentById( indexType, id );
        }

        /// <summary>
        /// Deletes the indexed documents.
        /// </summary>
        public void DeleteIndexedDocuments()
        {
            IndexContainer.DeleteDocumentsByType<EventItemIndex>();
        }

        /// <summary>
        /// Indexes the name of the model.
        /// </summary>
        /// <returns></returns>
        public Type IndexModelType()
        {
            return typeof( EventItemIndex );
        }

        /// <summary>
        /// Gets the index filter values.
        /// </summary>
        /// <returns></returns>
        public ModelFieldFilterConfig GetIndexFilterConfig()
        {
            return new ModelFieldFilterConfig() { FilterLabel = "", FilterField = "" };

        }

        /// <summary>
        /// Gets the index filter field.
        /// </summary>
        /// <returns></returns>
        public bool SupportsIndexFieldFiltering()
        {
            return true;
        }

        #endregion
    }

    #region Entity Configuration

    /// <summary>
    /// EventItem Configuration class.
    /// </summary>
    public partial class EventItemConfiguration : EntityTypeConfiguration<EventItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventItemConfiguration" /> class.
        /// </summary>
        public EventItemConfiguration()
        {
            this.HasOptional( i => i.ApprovedByPersonAlias ).WithMany().HasForeignKey( i => i.ApprovedByPersonAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( p => p.Photo ).WithMany().HasForeignKey( p => p.PhotoId ).WillCascadeOnDelete( false );
        }
    }

    #endregion
}