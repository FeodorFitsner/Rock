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
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.Web.Cache;
using Rock.Lava;

namespace Rock.Model
{
    /// <summary>
    /// Represents a persisted WorkflowActivity in Rock
    /// </summary>
    [RockDomain( "Workflow" )]
    [Table( "WorkflowActivity" )]
    [DataContract]
    public partial class WorkflowActivity : Model<WorkflowActivity>
    {

        #region Entity Properties

        /// <summary>
        /// Gets or sets the WorkflowId of the <see cref="Rock.Model.Workflow"/> instance that is performing this WorkflowActivity.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the WorkflowId of the <see cref="Rock.Model.Workflow"/> instance that is performing this WorkflowActivity.
        /// </value>
        [DataMember]
        public int WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the ActivityTypeId of the <see cref="Rock.Model.WorkflowActivityType"/> that is being executed.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> representing the ActivityTypeID of the <see cref="Rock.Model.WorkflowActivity"/> that is being performed.
        /// </value>
        [DataMember]
        public int ActivityTypeId { get; set; }

        /// <summary>
        /// Gets or sets the assigned person alias identifier.
        /// </summary>
        /// <value>
        /// The assigned person alias identifier.
        /// </value>
        [DataMember]
        public int? AssignedPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the assigned group identifier.
        /// </summary>
        /// <value>
        /// The assigned group identifier.
        /// </value>
        [DataMember]
        public int? AssignedGroupId { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this WorkflowActivity was activated.
        /// </summary>
        /// <value>
        /// A <see cref="System.DateTime"/> representing the date and time that this WorkflowActivity was activated.
        /// </value>
        [DataMember]
        public DateTime? ActivatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the activated by activity identifier.
        /// </summary>
        /// <value>
        /// The activated by activity identifier.
        /// </value>
        [DataMember]
        public int? ActivatedByActivityId { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this WorkflowActivity was last processed.
        /// </summary>
        /// <value>
        /// A <see cref="System.DateTime"/> representing the date and time that this WorkflowActivity was last processed.
        /// </value>
        [DataMember]
        [NotAudited]
        public DateTime? LastProcessedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this WorkflowActivity completed.
        /// </summary>
        /// <value>
        /// A <see cref="System.DateTime"/> representing the date and time that this WorkflowActivity completed.
        /// </value>
        [DataMember]
        public DateTime? CompletedDateTime { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.Workflow"/> instance that is performing this WorkflowActivity.
        /// </summary>
        /// <value>
        /// The <see cref="Rock.Model.Workflow"/> instance that is performing this WorkflowActivity.
        /// </value>
        [LavaVisible]
        public virtual Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.WorkflowActivityType"/> that is being performed by this WorkflowActivity instance.
        /// </summary>
        /// <value>
        /// The <see cref="Rock.Model.WorkflowActivityType"/> that is being performed by this WorkflowActivity instance.
        /// </value>
        [LavaVisible]
        public virtual WorkflowActivityType ActivityType { get; set; }

        /// <summary>
        /// Gets the activity type cache.
        /// </summary>
        /// <value>
        /// The activity type cache.
        /// </value>
        [LavaVisible]
        public virtual WorkflowActivityTypeCache ActivityTypeCache
        {
            get
            {
                if ( ActivityTypeId > 0 )
                {
                    return WorkflowActivityTypeCache.Get( ActivityTypeId );
                }
                else if ( ActivityType != null )
                {
                    return WorkflowActivityTypeCache.Get( ActivityType.Id );
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the assigned person alias.
        /// </summary>
        /// <value>
        /// The assigned person alias.
        /// </value>
        [LavaVisible]
        public virtual PersonAlias AssignedPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the assigned group.
        /// </summary>
        /// <value>
        /// The assigned group.
        /// </value>
        [LavaVisible]
        public virtual Group AssignedGroup { get; set; }

        /// <summary>
        /// Gets a value indicating whether this WorkflowActivity instance is active.
        /// </summary>
        /// <value>
        ///  A <see cref="System.Boolean"/> value that is <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [NotMapped]
        public virtual bool IsActive
        {
            get
            {
                return ( this.ActivityType?.IsActive ?? true ) && ActivatedDateTime.HasValue && !CompletedDateTime.HasValue;
            }
            private set { }
        }

        /// <summary>
        /// Gets or sets the activated by activity.
        /// </summary>
        /// <value>
        /// The activated by activity.
        /// </value>
        [LavaVisible]
        public virtual WorkflowActivity ActivatedByActivity { get; set; }

        /// <summary>
        /// Gets or sets a collection containing the <see cref="Rock.Model.WorkflowAction">WorkflowActions</see> that are run by this WorkflowActivity.
        /// </summary>
        /// <value>
        /// A collection containing the <see cref="Rock.Model.WorkflowAction">WorkflowActions</see> that are being run by this WorkflowActivity.
        /// </value>
        [DataMember]
        public virtual ICollection<WorkflowAction> Actions
        {
            get { return _actions ?? ( _actions = new Collection<WorkflowAction>() ); }
            set { _actions = value; }
        }
        private ICollection<WorkflowAction> _actions;

        /// <summary>
        /// Gets an enumerable collection containing the active <see cref="Rock.Model.WorkflowAction">WorkflowActions</see> for this WorkflowActivity, ordered by their order property.
        /// </summary>
        /// <value>
        /// An enumerable collection containing the active <see cref="Rock.Model.WorkflowAction">WorkflowActions</see> for this WorkflowActivity.
        /// </value>
        [LavaVisible]
        public virtual IEnumerable<Rock.Model.WorkflowAction> ActiveActions
        {
            get
            {
                return this.Actions
                    .Where( a => a.IsActive && !a.CompletedDateTime.HasValue )
                    .ToList()
                    .OrderBy( a => a.ActionTypeCache.Order );
            }
        }

        /// <summary>
        /// Gets the parent security authority for this WorkflowAction.
        /// </summary>
        /// <value>
        /// The parent security authority for this Workflow action.
        /// </value>
        public override Security.ISecured ParentAuthority
        {
            get
            {
                return this.Workflow != null ? this.Workflow : base.ParentAuthority;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override object this[object key]
        {
            get
            {
                string propertyKey = key.ToStringSafe();
                if ( propertyKey == "ActivityType" )
                {
                    return ActivityTypeCache;
                }
                return base[key];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes this WorkflowAction
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="entity">The entity that work is being performed against.</param>
        /// <param name="errorMessages">A 
        /// <see cref="System.Collections.Generic.List{String}" /> that will contain any error messages that are
        /// returned while processing this WorkflowActivity</param>
        /// <returns>
        /// A <see cref="System.Boolean" /> value that is <c>true</c> if the WorkflowActivity processes successfully; otherwise <c>false</c>.
        /// </returns>
        internal virtual bool Process( RockContext rockContext, Object entity, out List<string> errorMessages )
        {
            AddLogEntry( "Processing..." );

            errorMessages = new List<string>();

            foreach ( var action in this.ActiveActions )
            {
                List<string> actionErrorMessages;
                bool actionSuccess = action.Process( rockContext, entity, out actionErrorMessages );
                if ( actionErrorMessages.Any() )
                {
                    errorMessages.Add( string.Format( "Error in Activity: {0}; Action: {1} ({2} action type)", this.ActivityTypeCache.Name, action.ActionTypeCache.Name, action.ActionTypeCache.WorkflowAction.EntityType.FriendlyName ) );
                    errorMessages.AddRange( actionErrorMessages );
                }

                // If action was not successful, exit
                if ( !actionSuccess )
                {
                    break;
                }

                // If action completed this activity, exit
                if ( !this.IsActive )
                {
                    break;
                }

                // If action completed this workflow, exit
                if ( this.Workflow == null || !this.Workflow.IsActive )
                {
                    break;
                }
            }

            this.LastProcessedDateTime = RockDateTime.Now;

            AddLogEntry( "Processing Complete" );

            if ( !this.ActiveActions.Any() )
            {
                MarkComplete();
            }

            return errorMessages.Count == 0;
        }

        /// <summary>
        /// Adds a <see cref="Rock.Model.WorkflowLog" /> entry.
        /// </summary>
        /// <param name="logEntry">A <see cref="System.String" /> representing the body of the log entry.</param>
        /// <param name="force">if set to <c>true</c> will ignore logging level and always add the entry.</param>
        public virtual void AddLogEntry( string logEntry, bool force = false )
        {

            if ( this.Workflow != null )
            {
                var workflowType = this.Workflow.WorkflowTypeCache;
                if ( force || (
                    workflowType != null && (
                    workflowType.LoggingLevel == WorkflowLoggingLevel.Activity ||
                    workflowType.LoggingLevel == WorkflowLoggingLevel.Action ) ) )
                {
                    string idStr = Id > 0 ? "(" + Id.ToString() + ")" : "";
                    this.Workflow.AddLogEntry( string.Format( "{0} Activity {1}: {2}", this.ToString(), idStr, logEntry ), force );
                }
            }
        }

        /// <summary>
        /// Marks this WorkflowActivity as complete.
        /// </summary>
        public virtual void MarkComplete()
        {
            CompletedDateTime = RockDateTime.Now;
            AddLogEntry( "Completed" );
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this WorkflowActivity.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this WorkflowActivity.
        /// </returns>
        public override string ToString()
        {
            var activityType = this.ActivityTypeCache;
            if ( activityType != null )
            {
                return activityType.ToStringSafe();
            }
            return base.ToString();
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Activates the specified WorkflowActivity
        /// </summary>
        /// <param name="activityTypeCache">The activity type cache.</param>
        /// <param name="workflow">The persisted <see cref="Rock.Model.Workflow" /> instance that this Workflow activity belongs to.</param>
        /// <returns>
        /// The activated <see cref="Rock.Model.WorkflowActivity" />.
        /// </returns>
        public static WorkflowActivity Activate( WorkflowActivityTypeCache activityTypeCache, Workflow workflow )
        {
            using ( var rockContext = new RockContext() )
            {
                return Activate( activityTypeCache, workflow, rockContext );
            }
        }

        /// <summary>
        /// Activates the specified WorkflowActivity
        /// </summary>
        /// <param name="activityTypeCache">The activity type cache.</param>
        /// <param name="workflow">The persisted <see cref="Rock.Model.Workflow" /> instance that this Workflow activity belongs to.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns>
        /// The activated <see cref="Rock.Model.WorkflowActivity" />.
        /// </returns>
        public static WorkflowActivity Activate( WorkflowActivityTypeCache activityTypeCache, Workflow workflow, RockContext rockContext )
        {
            var activity = new WorkflowActivity();
            activity.Workflow = workflow;
            activity.ActivityTypeId = activityTypeCache.Id;
            activity.ActivatedDateTime = RockDateTime.Now;
            activity.LoadAttributes( rockContext );

            activity.AddLogEntry( "Activated" );

            foreach ( var actionType in activityTypeCache.ActionTypes )
            {
                activity.Actions.Add( WorkflowAction.Activate( actionType, activity, rockContext ) );
            }

            workflow.Activities.Add( activity );

            return activity;
        }

        #endregion

    }

    #region Entity Configuration

    /// <summary>
    /// WorkflowActivity Configuration class.
    /// </summary>
    public partial class WorkflowActivityConfiguration : EntityTypeConfiguration<WorkflowActivity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowActivityConfiguration"/> class.
        /// </summary>
        public WorkflowActivityConfiguration()
        {
            this.HasRequired( a => a.Workflow ).WithMany( a => a.Activities ).HasForeignKey( a => a.WorkflowId ).WillCascadeOnDelete( true );
            this.HasRequired( a => a.ActivityType ).WithMany().HasForeignKey( a => a.ActivityTypeId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.AssignedPersonAlias ).WithMany().HasForeignKey( a => a.AssignedPersonAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.AssignedGroup ).WithMany().HasForeignKey( a => a.AssignedGroupId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.ActivatedByActivity ).WithMany().HasForeignKey( a => a.ActivatedByActivityId ).WillCascadeOnDelete( false );
        }
    }

    #endregion

}

