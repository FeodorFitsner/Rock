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
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

using System.Runtime.Serialization;
using Rock.Data;
using Rock.Tasks;
using System.Linq;
using System.Data.Entity;

namespace Rock.Model
{
    /// <summary>
    /// Represents a step in Rock.
    /// </summary>
    [RockDomain( "Engagement" )]
    [Table( "Step" )]
    [DataContract]
    public partial class Step : Model<Step>, IOrdered
    {
        /* Custom Indexes:
         *
         * PersonAliasId, StepTypeId
         *      Includes CompletedDateTime
         *      This was added for Step Program Achievement
         *      
         *  StepTypeId, PersonAliasId
         *      Includes CompletedDateTime
         *      This was added for Step Program Achievement
         */

        #region Entity Properties

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.StepType"/> to which this step belongs. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int StepTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.StepStatus"/> to which this step belongs.
        /// </summary>
        [DataMember]
        public int? StepStatusId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.PersonAlias"/> that identifies the Person associated with taking this step. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int PersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.Campus"/> associated with this step.
        /// </summary>
        [DataMember]
        public int? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the completion of this step.
        /// </summary>
        [DataMember]
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the start of this step.
        /// </summary>
        [DataMember]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the end of this step.
        /// </summary>
        [DataMember]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// Gets the start date key.
        /// </summary>
        /// <value>
        /// The start date key.
        /// </value>
        [DataMember]
        [FieldType( Rock.SystemGuid.FieldType.DATE )]
        public int? StartDateKey
        {
            get => ( StartDateTime == null || StartDateTime.Value == default ) ?
                        ( int? ) null :
                        StartDateTime.Value.ToString( "yyyyMMdd" ).AsInteger();
            private set { }
        }

        /// <summary>
        /// Gets the end date key.
        /// </summary>
        /// <value>
        /// The end date key.
        /// </value>
        [DataMember]
        [FieldType( Rock.SystemGuid.FieldType.DATE )]
        public int? EndDateKey
        {
            get => ( EndDateTime == null || EndDateTime.Value == default ) ?
                        ( int? ) null :
                        EndDateTime.Value.ToString( "yyyyMMdd" ).AsInteger();
            private set { }
        }

        /// <summary>
        /// Gets the completed date key.
        /// </summary>
        /// <value>
        /// The completed date key.
        /// </value>
        [DataMember]
        [FieldType( Rock.SystemGuid.FieldType.DATE )]
        public int? CompletedDateKey
        {
            get => ( CompletedDateTime == null || CompletedDateTime.Value == default ) ?
                        ( int? ) null :
                        CompletedDateTime.Value.ToString( "yyyyMMdd" ).AsInteger();
            private set { }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Rock.Model.StepProgramCompletion"/> to which this step belongs.
        /// </summary>
        [DataMember]
        public int? StepProgramCompletionId { get; set; }

        #endregion Entity Properties

        #region IOrdered

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        #endregion IOrdered

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.StepType"/>.
        /// </summary>
        [DataMember]
        public virtual StepType StepType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.StepStatus"/>.
        /// </summary>
        [DataMember]
        public virtual StepStatus StepStatus { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.PersonAlias"/>.
        /// </summary>
        [DataMember]
        public virtual PersonAlias PersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.Campus"/>.
        /// </summary>
        [DataMember]
        public virtual Campus Campus { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.StepProgramCompletion"/>.
        /// </summary>
        [DataMember]
        public virtual StepProgramCompletion StepProgramCompletion { get; set; }

        /// <summary>
        /// Gets or sets a collection containing the <see cref="Rock.Model.StepWorkflow">StepWorkflows</see> that are of this step.
        /// </summary>
        [DataMember]
        public virtual ICollection<StepWorkflow> StepWorkflows
        {
            get => _stepWorkflows ?? ( _stepWorkflows = new Collection<StepWorkflow>() );
            set => _stepWorkflows = value;
        }
        private ICollection<StepWorkflow> _stepWorkflows;

        /// <summary>
        /// Indicates if this step has been completed
        /// </summary>
        [DataMember]
        public virtual bool IsComplete
        {
            get => StepStatus != null && StepStatus.IsCompleteStatus;
        }

        /// <summary>
        /// Gets or sets the start source date.
        /// </summary>
        /// <value>
        /// The start source date.
        /// </value>
        [DataMember]
        public virtual AnalyticsSourceDate StartSourceDate { get; set; }

        /// <summary>
        /// Gets or sets the end source date.
        /// </summary>
        /// <value>
        /// The end source date.
        /// </value>
        [DataMember]
        public virtual AnalyticsSourceDate EndSourceDate { get; set; }

        /// <summary>
        /// Gets or sets the completed source date.
        /// </summary>
        /// <value>
        /// The completed source date.
        /// </value>
        [DataMember]
        public virtual AnalyticsSourceDate CompletedSourceDate { get; set; }
        #endregion Virtual Properties

        #region Overrides

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            RockContext rockContext = null;

            // Get PersonAlias.
            var person = this.PersonAlias?.Person;
            if ( person == null && this.PersonAliasId != 0 )
            {
                if ( rockContext == null )
                {
                    rockContext = new RockContext();
                }
                var personAlias = new PersonAliasService( rockContext ).Get( this.PersonAliasId );
                person = personAlias?.Person;
            }

            // Get StepType.
            var stepType = this.StepType;
            if ( stepType == null )
            {
                if ( rockContext == null )
                {
                    rockContext = new RockContext();
                }
                stepType = new StepTypeService( rockContext ).Get( this.StepTypeId );
            }

            if ( person != null && stepType != null )
            {
                if ( stepType.AllowMultiple )
                {
                    // If "AllowMultiple" is set, include the Order to distinguish this from duplicate steps.
                    return $"{stepType.Name} {this.Order} - {person.FullName}";
                }

                return $"{stepType.Name} - {person.FullName}";
            }

            return base.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                var isValid = base.IsValid;

                if ( StartDateTime.HasValue && EndDateTime.HasValue && StartDateTime.Value > EndDateTime.Value )
                {
                    ValidationResults.Add( new ValidationResult( "The StartDateTime must occur before the EndDateTime" ) );
                    isValid = false;
                }

                if ( StartDateTime.HasValue && CompletedDateTime.HasValue && StartDateTime.Value > CompletedDateTime.Value )
                {
                    ValidationResults.Add( new ValidationResult( "The StartDateTime must occur before the CompletedDateTime" ) );
                    isValid = false;
                }

                return isValid;
            }
        }

        /// <summary>
        /// Perform tasks prior to saving changes to this entity.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entry">The entry.</param>
        public override void PreSaveChanges( Rock.Data.DbContext dbContext, DbEntityEntry entry )
        {
            int? previousStepStatusId = null;

            if ( entry.State == System.Data.Entity.EntityState.Modified )
            {
                var dbProperty = entry.Property( nameof( StepStatusId ) );
                previousStepStatusId = dbProperty?.OriginalValue as int?;
            }

            // Send a task to process workflows associated with changes to this Step.
            new LaunchStepChangeWorkflows.Message
            {
                EntityGuid = Guid,
                EntityState = entry.State,
                StepTypeId = StepTypeId,
                CurrentStepStatusId = StepStatusId,
                PreviousStepStatusId = previousStepStatusId
            }.Send();

            base.PreSaveChanges( dbContext, entry );
        }

        /// <summary>
        /// Posts the save changes.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public override void PostSaveChanges( Data.DbContext dbContext )
        {
            base.PostSaveChanges( dbContext );

            UpdateStepProgramCompletion( dbContext as RockContext );
        }

        private void UpdateStepProgramCompletion( RockContext rockContext )
        {
            if ( !this.CompletedDateTime.HasValue )
            {
                return;
            }

            var stepTypeService = new StepTypeService( rockContext );
            var stepType = this.StepType ?? stepTypeService.Get( this.StepTypeId );

            if ( stepType == null )
            {
                return;
            }

            var programStepTypeIds = stepTypeService
                .Queryable()
                .Where( a => a.StepProgramId == stepType.StepProgramId && a.IsActive )
                .Select( a => a.Id )
                .ToList();

            var steps = new StepService( rockContext )
                .Queryable()
                .AsNoTracking()
                .Where( a => a.PersonAliasId == this.PersonAliasId && !a.StepProgramCompletionId.HasValue && a.CompletedDateTime.HasValue )
                .OrderBy( a => a.CompletedDateTime )
                .ToList();

            while ( steps.Any() && programStepTypeIds.All( a => steps.Any( b => b.StepTypeId == a ) ) )
            {
                var stepSet = new List<Step>();
                foreach ( var programStepTypeId in programStepTypeIds )
                {
                    var step = steps.Where( a => a.StepTypeId == programStepTypeId ).FirstOrDefault();
                    if ( step == null )
                    {
                        continue;
                    }

                    stepSet.Add( step );
                    steps.RemoveAll( a => a.Id == step.Id );
                }

                StepService.UpdateStepProgramCompletion( stepSet, PersonAliasId, stepType.StepProgramId, rockContext );
            }
        }

        #endregion Overrides

        #region Entity Configuration

        /// <summary>
        /// Step Configuration class.
        /// </summary>
        public partial class StepConfiguration : EntityTypeConfiguration<Step>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StepConfiguration"/> class.
            /// </summary>
            public StepConfiguration()
            {
                HasRequired( s => s.StepType ).WithMany( st => st.Steps ).HasForeignKey( s => s.StepTypeId ).WillCascadeOnDelete( true );
                HasRequired( s => s.PersonAlias ).WithMany().HasForeignKey( s => s.PersonAliasId ).WillCascadeOnDelete( true );

                HasOptional( s => s.Campus ).WithMany().HasForeignKey( s => s.CampusId ).WillCascadeOnDelete( false );
                HasOptional( s => s.StepStatus ).WithMany( ss => ss.Steps ).HasForeignKey( s => s.StepStatusId ).WillCascadeOnDelete( false );
                HasOptional( s => s.StepProgramCompletion ).WithMany( ss => ss.Steps ).HasForeignKey( s => s.StepProgramCompletionId ).WillCascadeOnDelete( true );

                // NOTE: When creating a migration for this, don't create the actual FK's in the database for this just in case there are outlier OccurrenceDates that aren't in the AnalyticsSourceDate table
                // and so that the AnalyticsSourceDate can be rebuilt from scratch as needed
                this.HasOptional( r => r.StartSourceDate ).WithMany().HasForeignKey( r => r.StartDateKey ).WillCascadeOnDelete( false );
                this.HasOptional( r => r.EndSourceDate ).WithMany().HasForeignKey( r => r.EndDateKey ).WillCascadeOnDelete( false );
                this.HasOptional( r => r.CompletedSourceDate ).WithMany().HasForeignKey( r => r.CompletedDateKey ).WillCascadeOnDelete( false );
            }
        }

        #endregion
    }
}
