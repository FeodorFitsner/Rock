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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Rock.Model;
using Rock.Rest.Filters;
using Rock.Web.Cache;

namespace Rock.Rest.Controllers
{
    /// <summary>
    /// Blocks REST API
    /// </summary>
    public partial class BlocksController
    {
        /// <summary>
        /// Deletes the specified Block with extra logic to flush caches.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Authenticate, Secured]
        public override void Delete( int id )
        {
            SetProxyCreation( true );

            // get the ids of the page and layout so we can flush stuff after the base.Delete
            int? pageId = null;
            int? layoutId = null;
            int? siteId = null;

            var block = this.Get( id );
            if ( block != null )
            {
                pageId = block.PageId;
                layoutId = block.LayoutId;
                siteId = block.SiteId;
            }

            base.Delete( id );
        }

        /// <summary>
        /// Moves a block from one zone to another
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="block">The block.</param>
        /// <exception cref="HttpResponseException">
        /// </exception>
        [Authenticate, Secured]
        [HttpPut]
        [System.Web.Http.Route( "api/blocks/move/{id}" )]
        public void Move( int id, Block block )
        {
            var person = GetPerson();

            SetProxyCreation( true );

            block.Id = id;
            Block model;
            if ( !Service.TryGet( id, out model ) )
            {
                throw new HttpResponseException( HttpStatusCode.NotFound );
            }

            CheckCanEdit( model, person );

            model.Zone = block.Zone;
            model.PageId = block.PageId;
            model.LayoutId = block.LayoutId;
            model.SiteId = block.SiteId;

            if ( model.IsValid )
            {
                model.Order = ( (BlockService)Service ).GetMaxOrder( model );
                System.Web.HttpContext.Current.AddOrReplaceItem( "CurrentPerson", GetPerson() );
                Service.Context.SaveChanges();
            }
            else
            {
                throw new HttpResponseException( HttpStatusCode.BadRequest );
            }
        }

        /// <summary>
        /// Executes an action handler on a specific block.
        /// </summary>
        /// <param name="blockGuid">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        [Authenticate]
        [HttpGet]
        [Route( "api/blocks/action/{blockGuid}/{actionName}" )]
        public IHttpActionResult BlockAction( Guid blockGuid, string actionName )
        {
            return ProcessAction( HttpMethod.Get, blockGuid, actionName, null );
        }

        /// <summary>
        /// Executes an action handler on a specific block.
        /// </summary>
        /// <param name="blockGuid">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [Authenticate]
        [HttpPost]
        [Route( "api/blocks/action/{blockGuid}/{actionName}" )]
        public IHttpActionResult BlockActionAsPost( string blockGuid, string actionName, [NakedBody] string parameters )
        {
            if ( parameters == string.Empty )
            {
                return ProcessAction( Request.Method, blockGuid.AsGuidOrNull(), actionName, null );
            }

            //
            // We have to manually parse the JSON data, otherwise any strings
            // that look like dates get converted to Date objects. This causes
            // problems because then when we later stuff that Date object into
            // an actual string, the format has been changed. This happens, for
            // example, with Attribute Values.
            //
            using ( var stringReader = new StringReader( parameters ) )
            {
                using ( var jsonReader = new JsonTextReader( stringReader ) { DateParseHandling = DateParseHandling.None } )
                {
                    var parameterToken = JToken.ReadFrom( jsonReader );

                    return ProcessAction( Request.Method, blockGuid.AsGuidOrNull(), actionName, parameterToken );
                }
            }
        }

        /// <summary>
        /// Executes an action handler on a specific block.
        /// </summary>
        /// <param name="page">The page unique identifier.</param>
        /// <param name="block">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        /// <remarks>Use api/blocks/action/{blockGuid}/{actionName} instead, used by Rock Mobile shell v1 only.</remarks>
        [Authenticate]
        [HttpGet]
        [Route( "api/blocks/action/{page}/{block}/{actionName}")]
        [RockObsolete( "1.12" )]
        public IHttpActionResult BlockAction( string page, string block, string actionName )
        {
            return ProcessAction( HttpMethod.Get, block.AsGuidOrNull(), actionName, null );
        }

        /// <summary>
        /// Executes an action handler on a specific block.
        /// </summary>
        /// <param name="pageIdentifier">The page unique identifier.</param>
        /// <param name="blockIdentifier">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks>Use api/blocks/action/{blockGuid}/{actionName} instead, used by Rock Mobile shell v1 only.</remarks>
        [Authenticate]
        [Route( "api/blocks/action/{pageIdentifier}/{blockIdentifier}/{actionName}" )]
        [RockObsolete( "1.12" )]
        public IHttpActionResult BlockAction( string pageIdentifier, string blockIdentifier, string actionName, [NakedBody] string parameters )
        {
            //
            // We have to manually parse the JSON data, otherwise any strings
            // that look like dates get converted to Date objects. This causes
            // problems because then when we later stuff that Date object into
            // an actual string, the format has been changed. This happens, for
            // example, with Attribute Values.
            //
            using ( var stringReader = new StringReader( parameters ) )
            {
                using ( var jsonReader = new JsonTextReader( stringReader ) { DateParseHandling = DateParseHandling.None } )
                {
                    var parameterToken = JToken.ReadFrom( jsonReader );

                    return ProcessAction( Request.Method, blockIdentifier.AsGuidOrNull(), actionName, parameterToken );
                }
            }
        }

        /// <summary>
        /// Executes an action handler on a specific block.
        /// </summary>
        /// <param name="pageIdentifier">The page unique identifier.</param>
        /// <param name="blockIdentifier">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [RockObsolete( "1.11" )]
        [Authenticate]
        public IHttpActionResult BlockAction( string pageIdentifier, string blockIdentifier, string actionName, [FromBody] JToken parameters )
        {
            return ProcessAction( Request.Method, blockIdentifier.AsGuidOrNull(), actionName, parameters );
        }

        /// <summary>
        /// Processes the action.
        /// </summary>
        /// <param name="method">The HTTP Method Verb that was used for the request.</param>
        /// <param name="blockGuid">The block unique identifier.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private IHttpActionResult ProcessAction( HttpMethod method, Guid? blockGuid, string actionName, JToken parameters )
        {
            try
            {
                BlockCache blockCache = null;

                //
                // Find the block.
                //
                if ( blockGuid.HasValue )
                {
                    blockCache = BlockCache.Get( blockGuid.Value );
                }

                if ( blockCache == null )
                {
                    return NotFound();
                }

                //
                // Get the authenticated person and make sure it's cached.
                //
                var person = GetPerson();
                HttpContext.Current.AddOrReplaceItem( "CurrentPerson", person );

                //
                // Ensure the user has access to both the page and block.
                //
                if ( !blockCache.Page.IsAuthorized( Security.Authorization.VIEW, person ) || !blockCache.IsAuthorized( Security.Authorization.VIEW, person ) )
                {
                    return StatusCode( HttpStatusCode.Unauthorized );
                }

                //
                // Get the class that handles the logic for the block.
                //
                var blockCompiledType = blockCache.BlockType.GetCompiledType();
                var block = Activator.CreateInstance( blockCompiledType );

                if ( !( block is Blocks.IRockBlockType rockBlock ) )
                {
                    return NotFound();
                }

                var requestContext = new Net.RockRequestContext( Request );
                requestContext.AddContextEntitiesForPage( blockCache.Page );

                //
                // Set the basic block parameters.
                //
                rockBlock.BlockCache = blockCache;
                rockBlock.PageCache = blockCache.Page;
                rockBlock.RequestContext = requestContext;

                var actionParameters = new Dictionary<string, JToken>( StringComparer.InvariantCultureIgnoreCase );

                //
                // Parse any query string parameter data.
                //
                foreach ( var q in Request.GetQueryNameValuePairs() )
                {
                    actionParameters.AddOrReplace( q.Key, JToken.FromObject( q.Value.ToString() ) );
                }

                return InvokeAction( rockBlock, method, actionName, actionParameters, parameters );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        /// <summary>
        /// Processes the specified block action.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="method">The HTTP Method Verb that was used for the request.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="actionParameters">The action parameters.</param>
        /// <param name="bodyParameters">The posted body parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">actionName
        /// or
        /// actionData</exception>
        private IHttpActionResult InvokeAction( Blocks.IRockBlockType block, HttpMethod method, string actionName, Dictionary<string, JToken> actionParameters, JToken bodyParameters )
        {
            MethodInfo action;

            //
            // Find the action they requested. First search by name
            // and then further filter by any method constraint attributes.
            //
            action = block.GetType().GetMethods( BindingFlags.Instance | BindingFlags.Public )
                .Where( m => m.GetCustomAttribute<Blocks.BlockActionAttribute>()?.ActionName == actionName )
                .Where( m =>
                {
                    var attribs = m.GetCustomAttributes()
                        .Where( a => a is IActionHttpMethodProvider )
                        .Cast<IActionHttpMethodProvider>();

                    return !attribs.Any() || attribs.Any( a => a.HttpMethods.Contains( method ) );
                } )
                .SingleOrDefault();

            if ( action == null )
            {
                return NotFound();
            }

            var methodParameters = action.GetParameters();
            var parameters = new List<object>();

            // If we have posted body content then check if we have any
            // FromBody parameters, if not then parse the body content
            // into our normal parameters.
            if ( bodyParameters != null )
            {
                if ( !methodParameters.Any( a => a.GetCustomAttribute<FromBodyAttribute>() != null ) )
                {
                    try
                    {
                        // Parse any posted parameter data, existing query string
                        // parameters take precedence.
                        foreach ( var kvp in bodyParameters.ToObject<Dictionary<string, JToken>>() )
                        {
                            actionParameters.AddOrIgnore( kvp.Key, kvp.Value );
                        }
                    }
                    catch
                    {
                        return BadRequest( "Invalid parameter data." );
                    }
                }
            }

            //
            // Go through each parameter and convert it to the proper type.
            //
            for ( int i = 0; i < methodParameters.Length; i++ )
            {
                // Check if this parameter is requesting it's content from the body.
                if ( methodParameters[i].GetCustomAttribute<FromBodyAttribute>() != null )
                {
                    if ( bodyParameters != null )
                    {
                        parameters.Add( bodyParameters.ToObject( methodParameters[i].ParameterType ) );
                    }
                    else if ( methodParameters[i].IsOptional )
                    {
                        parameters.Add( Type.Missing );
                    }
                    else
                    {
                        return BadRequest( $"Parameter '{methodParameters[i].Name}' is required." );
                    }

                    continue;
                }

                var key = actionParameters.Keys.SingleOrDefault( k => k.ToLowerInvariant() == methodParameters[i].Name.ToLower() );

                if ( key != null )
                {
                    try
                    {
                        //
                        // If the target type is nullable and the action parameter is an empty
                        // string then consider it null. A GET query cannot have null values.
                        //
                        if ( Nullable.GetUnderlyingType( methodParameters[i].ParameterType ) != null )
                        {
                            if ( actionParameters[key].Type == JTokenType.String && actionParameters[key].ToString() == string.Empty )
                            {
                                parameters.Add( null );

                                continue;
                            }
                        }

                        parameters.Add( actionParameters[key].ToObject( methodParameters[i].ParameterType ) );
                    }
                    catch
                    {
                        return BadRequest( $"Parameter type mismatch for '{methodParameters[i].Name}'." );
                    }
                }
                else if ( methodParameters[i].IsOptional )
                {
                    parameters.Add( Type.Missing );
                }
                else
                {
                    return BadRequest( $"Parameter '{methodParameters[i].Name}' is required." );
                }
            }

            object result;
            try
            {
                result = action.Invoke( block, parameters.ToArray() );
            }
            catch ( TargetInvocationException ex )
            {
                ExceptionLogService.LogApiException( ex.InnerException, Request, GetPersonAlias() );
                result = new Rock.Blocks.BlockActionResult( HttpStatusCode.InternalServerError );
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogApiException( ex, Request, GetPersonAlias() );
                result = new Rock.Blocks.BlockActionResult( HttpStatusCode.InternalServerError );
            }

            //
            // Handle the result type.
            //
            if ( result is IHttpActionResult )
            {
                return ( IHttpActionResult ) result;
            }
            else if ( result is Rock.Blocks.BlockActionResult actionResult )
            {
                if ( actionResult.Error != null )
                {
                    return Content( actionResult.StatusCode, new HttpError( actionResult.Error ) );
                }
                else if ( actionResult.Content is HttpContent httpContent )
                {
                    var response = Request.CreateResponse( actionResult.StatusCode );
                    response.Content = httpContent;
                    return new System.Web.Http.Results.ResponseMessageResult( response );
                }
                else if ( actionResult.ContentClrType != null )
                {
                    var genericType = typeof( System.Web.Http.Results.NegotiatedContentResult<> ).MakeGenericType( actionResult.ContentClrType );
                    return ( IHttpActionResult ) Activator.CreateInstance( genericType, actionResult.StatusCode, actionResult.Content, this );
                }
                else
                {
                    return StatusCode( actionResult.StatusCode );
                }
            }
            else if ( action.ReturnType == typeof(void))
            {
                return Ok();
            }
            else
            {
                return Ok( result );
            }
        }
    }
}
